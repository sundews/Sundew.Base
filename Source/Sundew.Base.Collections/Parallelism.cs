namespace Sundew.Base.Collections;

using System;
using System.Threading.Tasks;

/// <summary>
/// Specifies configuration options for controlling parallel execution, including the maximum degree of parallelism, cancellation support, and task scheduling behavior.
/// </summary>
/// <remarks>Use this struct to configure parallel operations, such as controlling concurrency, enabling cancellation, or customizing task scheduling.
/// The configuration is immutable and can be reused across multiple parallel invocations. Thread safety is guaranteed due to its value type and readonly semantics.</remarks>
/// <param name="MaxDegreeOfParallelism">The maximum number of concurrent tasks allowed. Specify a value greater than zero to limit parallelism, or -1 to use the system default (typically the number of logical processors).</param>
/// <param name="Cancellation">A cancellation token or configuration that enables cooperative cancellation of parallel operations. Use <see cref="Cancellation.None"/> to disable cancellation.</param>
/// <param name="TaskScheduler">The task scheduler used to schedule parallel tasks. Specify <see langword="null"/> to use the default scheduler.</param>
public readonly record struct Parallelism(
    int MaxDegreeOfParallelism = -1,
    Cancellation Cancellation = default,
    TaskScheduler? TaskScheduler = null)
{
    /// <summary>
    /// Represents the default parallelism configuration, using the number of available processors, no cancellation, and
    /// the default task scheduler.
    /// </summary>
    /// <remarks>Use this value to perform parallel operations with system-default settings. The maximum
    /// degree of parallelism is set to the number of logical processors on the current machine. Cancellation is
    /// disabled, and tasks are scheduled using the default scheduler.</remarks>
    public static readonly Parallelism Default = new(
        MaxDegreeOfParallelism: Environment.ProcessorCount,
        Cancellation: Cancellation.None,
        TaskScheduler: TaskScheduler.Default);

    /// <summary>
    /// Represents a configuration that disables parallelism, executing tasks sequentially on the default task scheduler without cancellation support.
    /// </summary>
    /// <remarks>Use this value when operations must run one at a time, ensuring no concurrent execution. This
    /// is useful for scenarios where thread safety is a concern or when parallel execution is not desired.</remarks>
    public static readonly Parallelism None = new(
        MaxDegreeOfParallelism: 1,
        Cancellation: Cancellation.None,
        TaskScheduler: TaskScheduler.Default);
}