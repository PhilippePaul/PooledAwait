﻿using PooledAwait.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PooledAwait
{
    /// <summary>
    /// A task-source that automatically recycles when the task is awaited
    /// </summary>
    public readonly struct PooledValueTaskSource
    {
        /// <summary>
        /// Gets the task that corresponds to this instance; it can only be awaited once
        /// </summary>
        public ValueTask Task
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new PooledValueTask(_source, _token).AsValueTask();
        }

        /// <summary>
        /// Rents a task-source that will be recycled when the task is awaited
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledValueTaskSource Create()
        {
            var source = PooledState.Create(out var token);
            return new PooledValueTaskSource(source, token);
        }

        private readonly PooledState _source;
        private readonly short _token;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PooledValueTaskSource(PooledState source, short token)
        {
            _source = source;
            _token = token;
        }

        /// <summary>
        /// Test whether the source is valid
        /// </summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source != null && _source.IsValid(_token);
        }

        /// <summary>
        /// Set the result of the operation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetResult() => _source != null && _source.TrySetResult(_token);

        /// <summary>
        /// Set the result of the operation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetException(Exception error) => _source != null && _source.TrySetException(error, _token);
    }
}
