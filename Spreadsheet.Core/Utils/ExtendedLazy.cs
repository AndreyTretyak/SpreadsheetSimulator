// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma warning disable 0420

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
//
//
// --------------------------------------------------------------------------------------
//
// A class that provides a simple, lightweight implementation of lazy initialization, 
// obviating the need for a developer to implement a custom, thread-safe lazy initialization 
// solution.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Threading;
using Spreadsheet.Core.Exceptions;

namespace Spreadsheet.Core.Utils
{
    // Lazy<T> is generic, but not all of its state needs to be generic.  Avoid creating duplicate
    // objects per instantiation by putting them here.
    internal static class LazyHelpers
    {
        // Dummy object used as the value of m_threadSafeObj if in PublicationOnly mode.
        internal static readonly object PUBLICATION_ONLY_SENTINEL = new object();
    }

    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="TParameter">Specifies the type of parameter that will be passed to initialization function.</typeparam>
    /// <typeparam name="TResult">Specifies the type of element being lazily initialized.</typeparam>
    /// <remarks>
    /// <para>
    /// By default, all public and protected members of <see cref="ExtendedLazy{TResult,TParametr}"/> are thread-safe and may be used
    /// concurrently from multiple threads.  These thread-safety guarantees may be removed optionally and per instance
    /// using parameters to the type's constructors.
    /// </para>
    /// </remarks>
    public class ExtendedLazy<TParameter, TResult>
    {

        #region Inner classes
        /// <summary>
        /// wrapper class to box the initialized value, this is mainly created to avoid boxing/unboxing the value each time the value is called in case T is 
        /// a value type
        /// </summary>
        class Boxed
        {
            internal Boxed(TResult value)
            {
                m_value = value;
            }
            internal TResult m_value;
        }


        /// <summary>
        /// Wrapper class to wrap the exception thrown by the value factory
        /// </summary>
        class LazyInternalExceptionHolder
        {
            internal ExceptionDispatchInfo m_edi;
            internal LazyInternalExceptionHolder(Exception ex)
            {
                m_edi = ExceptionDispatchInfo.Capture(ex);
            }
        }
        #endregion

        // A dummy delegate used as a  :
        // 1- Flag to avoid recursive call to Value in None and ExecutionAndPublication modes in m_valueFactory
        // 2- Flag to m_threadSafeObj if ExecutionAndPublication mode and the value is known to be initialized
        static readonly Func<TParameter, TResult> ALREADY_INVOKED_SENTINEL = delegate
        {
            Contract.Assert(false, "ALREADY_INVOKED_SENTINEL should never be invoked.");
            return default(TResult);
        };

        //null --> value is not created
        //m_value is Boxed --> the value is created, and m_value holds the value
        //m_value is LazyExceptionHolder --> it holds an exception
        private object m_boxed;

        // The factory delegate that returns the value.
        [NonSerialized]
        private TParameter m_state;

        // The factory delegate that returns the value.
        [NonSerialized]
        private Func<TParameter, TResult> m_valueFactory;

        // object if ExecutionAndPublication mode (may be ALREADY_INVOKED_SENTINEL if the value is already initialized)
        [NonSerialized]
        private object m_threadSafeObj;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Threading.Lazy{T}"/> class
        /// that uses a specified initialization function and a specified thread-safety mode.
        /// </summary>
        /// <param name="valueFactory">
        /// The <see cref="T:System.Func{T}"/> invoked to produce the lazily-initialized value when it is needed.
        /// </param>
        /// <param name="mode">The lazy thread-safety mode.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="valueFactory"/> is
        /// a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="mode"/> mode contains an invalid value.</exception>
        public ExtendedLazy(TParameter state, Func<TParameter, TResult> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));
            m_state = state;
            m_threadSafeObj = new object();
            m_valueFactory = valueFactory;
        }


        /// <summary>Forces initialization during serialization.</summary>
        /// <param name="context">The StreamingContext for the serialization operation.</param>
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            // Force initialization
            TResult dummy = Value;
        }

        /// <summary>Creates and returns a string representation of this instance.</summary>
        /// <returns>The result of calling <see cref="System.Object.ToString"/> on the <see
        /// cref="Value"/>.</returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <see cref="Value"/> is null.
        /// </exception>
        public override string ToString()
        {
            return IsValueCreated ? Value.ToString() : ("Lazy_ToString_ValueNotCreated");
        }

        /// <summary>Gets the value of the Lazy&lt;T&gt; for debugging display purposes.</summary>
        internal TResult ValueForDebugDisplay
        {
            get
            {
                if (!IsValueCreated)
                {
                    return default(TResult);
                }
                return ((Boxed)m_boxed).m_value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance may be used concurrently from multiple threads.
        /// </summary>
        internal LazyThreadSafetyMode Mode
        {
            get
            {
                return LazyThreadSafetyMode.ExecutionAndPublication;
            }
        }

        /// <summary>
        /// Gets whether the value creation is faulted or not
        /// </summary>
        internal bool IsValueFaulted
        {
            get { return m_boxed is LazyInternalExceptionHolder; }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Lazy{T}"/> has been initialized.
        /// </summary>
        /// <value>true if the <see cref="T:System.Lazy{T}"/> instance has been initialized;
        /// otherwise, false.</value>
        /// <remarks>
        /// The initialization of a <see cref="T:System.Lazy{T}"/> instance may result in either
        /// a value being produced or an exception being thrown.  If an exception goes unhandled during initialization, 
        /// <see cref="IsValueCreated"/> will return false.
        /// </remarks>
        public bool IsValueCreated
        {
            get
            {
                return m_boxed != null && m_boxed is Boxed;
            }
        }

        /// <summary>Gets the lazily initialized value of the current <see
        /// cref="T:System.Threading.Lazy{T}"/>.</summary>
        /// <value>The lazily initialized value of the current <see
        /// cref="T:System.Threading.Lazy{T}"/>.</value>
        /// <exception cref="T:System.MissingMemberException">
        /// The <see cref="T:System.Threading.Lazy{T}"/> was initialized to use the default constructor 
        /// of the type being lazily initialized, and that type does not have a public, parameterless constructor.
        /// </exception>
        /// <exception cref="T:System.MemberAccessException">
        /// The <see cref="T:System.Threading.Lazy{T}"/> was initialized to use the default constructor 
        /// of the type being lazily initialized, and permissions to access the constructor were missing.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Threading.Lazy{T}"/> was constructed with the <see cref="T:System.Threading.LazyThreadSafetyMode.ExecutionAndPublication"/> or
        /// <see cref="T:System.Threading.LazyThreadSafetyMode.None"/>  and the initialization function attempted to access <see cref="Value"/> on this instance.
        /// </exception>
        /// <remarks>
        /// If <see cref="IsValueCreated"/> is false, accessing <see cref="Value"/> will force initialization.
        /// Please <see cref="System.Threading.LazyThreadSafetyMode"> for more information on how <see cref="T:System.Threading.Lazy{T}"/> will behave if an exception is thrown
        /// from initialization delegate.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public TResult Value
        {
            get
            {
                Boxed boxed = null;
                if (m_boxed != null)
                {
                    // Do a quick check up front for the fast path.
                    boxed = m_boxed as Boxed;
                    if (boxed != null)
                    {
                        return boxed.m_value;
                    }

                    LazyInternalExceptionHolder exc = m_boxed as LazyInternalExceptionHolder;
                    Contract.Assert(exc != null);
                    exc.m_edi.Throw();
                }

                // Fall through to the slow path.
#if !FEATURE_CORECLR
                // We call NOCTD to abort attempts by the debugger to funceval this property (e.g. on mouseover)
                //   (the debugger proxy is the correct way to look at state/value of this object)
                Debugger.NotifyOfCrossThreadDependency();
#endif
                return LazyInitValue();

            }
        }

        /// <summary>
        /// local helper method to initialize the value 
        /// </summary>
        /// <returns>The inititialized T value</returns>
        private TResult LazyInitValue()
        {
            Boxed boxed = null;

            object threadSafeObj = Volatile.Read(ref m_threadSafeObj);
            bool lockTaken = false;
            try
            {
                if (threadSafeObj != (object)ALREADY_INVOKED_SENTINEL)
                    Monitor.Enter(threadSafeObj, ref lockTaken);
                else
                    Contract.Assert(m_boxed != null);

                if (m_boxed == null)
                {
                    boxed = CreateValue();
                    m_boxed = boxed;
                    Volatile.Write(ref m_threadSafeObj, ALREADY_INVOKED_SENTINEL);
                }
                else // got the lock but the value is not null anymore, check if it is created by another thread or faulted and throw if so
                {
                    boxed = m_boxed as Boxed;
                    if (boxed == null) // it is not Boxed, so it is a LazyInternalExceptionHolder
                    {
                        LazyInternalExceptionHolder exHolder = m_boxed as LazyInternalExceptionHolder;
                        Contract.Assert(exHolder != null);
                        exHolder.m_edi.Throw();
                    }
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(threadSafeObj);
            }

            Contract.Assert(boxed != null);
            return boxed.m_value;
        }

        /// <summary>Creates an instance of T using m_valueFactory in case its not null or use reflection to create a new T()</summary>
        /// <returns>An instance of Boxed.</returns>
        private Boxed CreateValue()
        {
            Boxed boxed = null;
            try
            {
                // check for recursion
                if (m_valueFactory == ALREADY_INVOKED_SENTINEL)
                    throw new  CircularCellRefereceException(Resources.CircularReference);

                Func<TParameter, TResult> factory = m_valueFactory;
                m_valueFactory = ALREADY_INVOKED_SENTINEL;
                boxed = new Boxed(factory(m_state));
            }
            catch (Exception ex)
            {
                m_boxed = new LazyInternalExceptionHolder(ex);
                throw;
            }
            return boxed;
        }

    }
}