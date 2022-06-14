﻿// Copyright (c) 2022 Jonathan Lang
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Baracuda.Monitoring.Internal.Units;
using Baracuda.Monitoring.Internal.Utilities;
using Baracuda.Reflection;
using UnityEngine;

namespace Baracuda.Monitoring.Internal.Profiling
{
    internal static partial class ValueProcessorFactory
    {
        #region --- Find ValueProcessor ---

        /// <summary>
        /// This method will scan the declaring <see cref="Type"/> of the passed
        /// <see cref="ValueProfile{TTarget,TValue}"/> for a valid processor method with the passed name.<br/>
        /// Certain types offer special functionality and require additional handling. Those types are:<br/>
        /// Types assignable from <see cref="IList{T}"/> (inc. <see cref="Array"/>)<br/>
        /// Types assignable from <see cref="IDictionary{TKey, TValue}"/><br/>
        /// Types assignable from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="processor">name of the method declared as a value processor</param>
        /// <param name="valueProfile">the profile of the <see cref="ValueUnit{TTarget,TValue}"/></param>
        /// <typeparam name="TTarget">the <see cref="Type"/> of the profiles Target instance</typeparam>
        /// <typeparam name="TValue">the <see cref="Type"/> of the profiles value instance</typeparam>
        /// <returns></returns>
        internal static Func<TValue, string> FindCustomStaticProcessor<TTarget, TValue>(
            string processor,
            ValueProfile<TTarget, TValue> valueProfile) where TTarget : class
        {
            try
            {
                // validate that the processor name is not null.
                if (string.IsNullOrWhiteSpace(processor))
                {
                    return null;
                }

                // setup
                var declaringType = valueProfile.UnitTargetType;
                var valueType = valueProfile.UnitValueType;

                // get the processor method.
                var processorMethod = declaringType.GetMethod(processor, STATIC_FLAGS);

                // validate that the processor is not null.
                if (processorMethod == null)
                {
                    return null;
                }

                // create a delegate with for the processors method.
                var processorFunc = processorMethod.CreateMatchingDelegate(FLAGS);

                //----------------------------

                // cache the parameter information of the processor method.
                var parameterInfos = processorFunc.Method.GetParameters();

                if (!parameterInfos.Any())
                {
                    ExceptionLogging.LogInvalidProcessorSignature(processor, declaringType);
                    return null;
                }

                //----------------------------

                // Check if the types are a perfect match
                if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType.IsAssignableFrom(valueType))
                {
                    var defaultDelegate = (Func<TValue, string>) processorFunc;
                    return value => defaultDelegate(value);
                }

                //----------------------------

                #region --- Ilist<T> ---

                // IList<T> processor
                if (valueType.IsGenericIList())
                {
                    //Debug.Log("IList");
                    if (parameterInfos[0].ParameterType.IsAssignableFrom(valueType.IsArray
                            ? valueType.GetElementType()
                            : valueType.GetGenericArguments()[0]))
                    {
                        // IList<T> processor passing each element of the collection
                        if (parameterInfos.Length == 1)
                        {
                            // create a generic method to create the generic processor
                            var delegateCreationMethod = genericIListWithoutIndexCreateMethod
                                .MakeGenericMethod(valueType,
                                    valueType.IsArray
                                        ? valueType.GetElementType()
                                        : valueType.GetGenericArguments()[0]);

                            // create a delegate of type: <Func<TValue, string>> by invoking the delegateCreationMethod
                            var func = delegateCreationMethod
                                .Invoke(null, new object[] {processorFunc.Method, valueProfile.FormatData.Label})
                                ?.ConvertFast<object, Func<TValue, string>>();

                            // return the delegate if it was created successfully
                            if (func != null)
                            {
                                return func;
                            }
                        }

                        // IList<T> processor passing each element of the collection with the associated index.
                        if (parameterInfos.Length == 2 && parameterInfos[1].ParameterType == typeof(int))
                        {
                            // create a generic method to create the generic processor
                            var delegateCreationMethod = genericIListWithIndexCreateMethod
                                .MakeGenericMethod(valueType,
                                    valueType.IsArray
                                        ? valueType.GetElementType()
                                        : valueType.GetGenericArguments()[0]);

                            // create a delegate of type: <Func<TValue, string>> by invoking the delegateCreationMethod
                            var func = delegateCreationMethod
                                .Invoke(null, new object[] {processorFunc.Method, valueProfile.FormatData.Label})
                                ?.ConvertFast<object, Func<TValue, string>>();

                            // return the delegate if it was created successfully
                            if (func != null)
                            {
                                return func;
                            }
                        }
                    }
                }

                #endregion

                //----------------------------

                #region --- IDictionary<TKey,TValue> ---

                // IDictionary<TKey, TValue> processor
                if (valueType.IsGenericIDictionary())
                {
                    // Signature validation 1.
                    // check that the length of the signature is 2, so that it could potentially contain two
                    // valid arguments. (TKey and TValue)
                    if (parameterInfos.Length == 2)
                    {
                        // Signature validation 2.
                        // check that the signature of the processor method is compatible with the generic type definition
                        // of the dictionaries KeyValuePair<TKey, TValue> 
                        var genericArgs = valueType.GetGenericArguments();
                        if (parameterInfos[0].ParameterType == genericArgs[0]
                            && parameterInfos[1].ParameterType == genericArgs[1])
                        {
                            // create a generic method to create the generic processor
                            var delegateCreationMethod = genericIDictionaryCreateMethod
                                .MakeGenericMethod(valueType, genericArgs[0], genericArgs[1]);

                            // create a delegate of type: <Func<TValue, string>> by invoking the delegateCreationMethod
                            var func = delegateCreationMethod
                                .Invoke(null, new object[] {processorFunc.Method, valueProfile.FormatData.Label})
                                ?.ConvertFast<object, Func<TValue, string>>();

                            // return the delegate if it was created successfully
                            if (func != null)
                            {
                                return func;
                            }
                        }
                    }
                }

                #endregion

                //----------------------------

                #region --- IEnumerable<T> ---

                // IEnumerable<T> processor
                if (valueType.IsGenericIEnumerable(true))
                {
                    // check that the signature of the processor method is compatible with the generic type definition
                    // of the IEnumerable<T>s generic type definition.
                    if (parameterInfos.Length == 1 &&
                        parameterInfos[0].ParameterType == valueType.GetGenericArguments()[0])
                    {
                        // create a generic method to create the generic processor
                        var delegateCreationMethod = genericIEnumerableProcessorMethod
                            .MakeGenericMethod(valueType, valueType.GetGenericArguments()[0]);

                        // create a delegate of type: <Func<TValue, string>> by invoking the delegateCreationMethod
                        var func = delegateCreationMethod
                            .Invoke(null, new object[] {processorFunc.Method, valueProfile.FormatData.Label})
                            ?.ConvertFast<object, Func<TValue, string>>();

                        // return the delegate if it was created successfully
                        if (func != null)
                        {
                            return func;
                        }
                    }
                }

                #endregion

                //----------------------------

                return null;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }


        /*
         * Instance   
         */

        /// <summary>
        /// This method will scan the declaring <see cref="Type"/> of the passed
        /// <see cref="ValueProfile{TTarget,TValue}"/> for a valid processor method with the passed name.<br/>
        /// Certain types offer special functionality and require additional handling. Those types are:<br/>
        /// Types assignable from <see cref="IList{T}"/> (inc. <see cref="Array"/>)<br/>
        /// Types assignable from <see cref="IDictionary{TKey, TValue}"/><br/>
        /// Types assignable from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="processor">name of the method declared as a value processor</param>
        /// <param name="valueProfile">the profile of the <see cref="ValueUnit{TTarget,TValue}"/></param>
        /// <typeparam name="TTarget">the <see cref="Type"/> of the profiles Target instance</typeparam>
        /// <typeparam name="TValue">the <see cref="Type"/> of the profiles value instance</typeparam>
        /// <returns></returns>
        internal static Func<TTarget, TValue, string> FindCustomInstanceProcessor<TTarget, TValue>(
            string processor, ValueProfile<TTarget, TValue> valueProfile) where TTarget : class
        {
            try
            {
                // validate that the processor name is not null.
                if (string.IsNullOrWhiteSpace(processor))
                {
                    return null;
                }

                var declaringType = valueProfile.UnitTargetType;
                var valueType = valueProfile.UnitValueType;

                var processorMethod = declaringType.GetMethod(processor, INSTANCE_FLAGS);

                if (processorMethod == null)
                {
                    return null;
                }

                // create a delegate with for the processors method.
                var processorFunc =
                    (Func<TTarget, TValue, string>) processorMethod.CreateDelegate(
                        typeof(Func<TTarget, TValue, string>));

                //----------------------------

                // cache the parameter information of the processor method.
                var parameterInfos = processorFunc.Method.GetParameters();

                if (!parameterInfos.Any())
                {
                    ExceptionLogging.LogInvalidProcessorSignature(processor, declaringType);
                    return null;
                }

                //----------------------------

                // Check if the types are a perfect match
                if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType.IsAssignableFrom(valueType))
                {
                    return (target, value) => processorFunc(target, value);
                }

                //----------------------------

                return null;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- IEnumerable ---

        private static readonly MethodInfo genericIEnumerableProcessorMethod =
            typeof(ValueProcessorFactory).GetMethod(nameof(CreateIEnumerableFunc), STATIC_FLAGS);

        private static Func<TInput, string> CreateIEnumerableFunc<TInput, TElement>(MethodInfo processor, string name)
            where TInput : IEnumerable<TElement>
        {
            try
            {
                // create a matching delegate with the processors method info.
                var enumerableDelegate =
                    (Func<TElement, string>) Delegate.CreateDelegate(typeof(Func<TElement, string>), processor);

                // create a matching null string.
                var nullString = $"{name}: {NULL}";

                // create a stringBuilder object to be used by the lambda.
                var sb = new StringBuilder();

                //Processor Code
                return (value) =>
                {
                    // check that the passed value is not null.
                    if ((object) value != null)
                    {
                        sb.Clear();
                        sb.Append(name);
                        foreach (TElement element in value)
                        {
                            sb.Append(Environment.NewLine);
                            sb.Append(DEFAULT_INDENT);
                            sb.Append(enumerableDelegate(element));
                        }

                        return sb.ToString();
                    }

                    return nullString;
                };
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return null;
        }
        
        #endregion
        
        #region --- Dictionary ---
        
        private static readonly MethodInfo genericIDictionaryCreateMethod =
            typeof(ValueProcessorFactory).GetMethod(nameof(CreateIDictionaryFunc), STATIC_FLAGS);

        private static Func<TInput, string> CreateIDictionaryFunc<TInput, TKey, TValue>(MethodInfo processor, string name) where TInput : IDictionary<TKey, TValue>
        {
            try
            {
                // create a matching delegate with the processors method info.
                var genericFunc = (Func<TKey,TValue,string>)Delegate.CreateDelegate(typeof(Func<TKey,TValue,string>), processor);

                // create a matching null string.
                var nullString = $"{name}: {NULL}";
                
                // create a stringBuilder object to be used by the lambda.
                var stringBuilder = new StringBuilder();
                
                return (value) =>
                {
                    // check that the passed value is not null.
                    if (value != null)
                    {
                        stringBuilder.Clear();
                        stringBuilder.Append(name);
                        
                        foreach (KeyValuePair<TKey, TValue> valuePair in value)
                        {
                            stringBuilder.Append(Environment.NewLine);
                            stringBuilder.Append(DEFAULT_INDENT);
                            stringBuilder.Append(genericFunc(valuePair.Key, valuePair.Value));
                        }
                        return stringBuilder.ToString();
                    }

                    return nullString;
                };

            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return null;
        }

        #endregion
        
        #region --- IList ---

        private static readonly MethodInfo genericIListWithoutIndexCreateMethod =
            typeof(ValueProcessorFactory).GetMethod(nameof(CreateIListFuncWithoutIndexArgument), STATIC_FLAGS);
        
        private static Func<TInput, string> CreateIListFuncWithoutIndexArgument<TInput, TElement>(MethodInfo processor, string name) where TInput : IList<TElement>
        {
            try
            {
                // create a matching delegate with the processors method info.
                var listDelegate = (Func<TElement, string>)Delegate.CreateDelegate(typeof(Func<TElement, string>), processor);
                
                // create a matching null string.
                var nullString = $"{name}: {NULL}";

                // create a stringBuilder object to be used by the lambda.
                var stringBuilder = new StringBuilder();

                #region --- Processor Code ---

                return (value) =>
                {
                    // check that the passed value is not null.
                    if (value != null)
                    {
                        stringBuilder.Clear();
                        stringBuilder.Append(name);
                        
                        for (int i = 0; i < value.Count; i++)
                        {
                            stringBuilder.Append(Environment.NewLine);
                            stringBuilder.Append(DEFAULT_INDENT);
                            stringBuilder.Append(listDelegate(value[i]));
                        }
                        return stringBuilder.ToString();
                    }

                    return nullString;
                };

                #endregion 
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return null;
        }

        #endregion
        
        #region --- IList + Index ---

        private static readonly MethodInfo genericIListWithIndexCreateMethod =
            typeof(ValueProcessorFactory).GetMethod(nameof(CreateIListFuncWithIndexArgument), STATIC_FLAGS);
        
        /// <summary>
        /// Creates a delegate that accepts an input of type <see cref="IList{TElement}"/> and returns a string, using
        /// a custom value processor with a signature <see cref="string"/>(<see cref="TElement"/> element <see cref="Int32"/> index)<br/>
        /// </summary>
        /// <param name="processor">the <see cref="MethodInfo"/> of the previously validated processor</param>
        /// <param name="name">the name of the <see cref="ValueUnit{TTarget,TValue}"/></param>
        /// <typeparam name="TInput">the exact argument/input type of the processors method. This type must be assignable from <see cref="IList{TElement}"/></typeparam>
        /// <typeparam name="TElement">the element type of the IList</typeparam>
        /// <returns></returns>
        private static Func<TInput, string> CreateIListFuncWithIndexArgument<TInput, TElement>(MethodInfo processor, string name) where TInput : IList<TElement>
        {
            try
            {
                // create a matching delegate with the processors method info.
                var listDelegate = (Func<TElement, int, string>)Delegate.CreateDelegate(typeof(Func<TElement, int, string>), processor);
                
                // create a matching null string.
                var nullString = $"{name}: {NULL}";
                
                // create a stringBuilder object to be used by the lambda.
                var stringBuilder = new StringBuilder();

                #region --- Processor Code ---

                return (value) =>
                {
                    // check that the passed value is not null.
                    if (value != null)
                    {
                        stringBuilder.Clear();
                        stringBuilder.Append(name);
                        for (int i = 0; i < value.Count; i++)
                        {
                            stringBuilder.Append(Environment.NewLine);
                            stringBuilder.Append(DEFAULT_INDENT);
                            stringBuilder.Append(listDelegate(value[i], i));
                        }
                        return stringBuilder.ToString();
                    }

                    return nullString;
                };

                #endregion 
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return null;
        }

        #endregion
    }
}