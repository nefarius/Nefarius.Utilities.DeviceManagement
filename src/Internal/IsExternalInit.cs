#if !NET5_0_OR_GREATER
using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
///     Polyfill that enables <c>record</c> types and <c>init</c> accessors on older target frameworks.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit
{
}
#endif