using System;

namespace Isotope.Areas.Admin.Utils;

/// <summary>
/// Internal exception indicating that an operation failed to complete.
/// </summary>
public class OperationException(string message) : Exception(message);