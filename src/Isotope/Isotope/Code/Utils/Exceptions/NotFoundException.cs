using System;

namespace Isotope.Code.Utils.Exceptions;

/// <summary>
/// Exception for indicating that a resource does not exist.
/// </summary>
public class NotFoundException(string message) : Exception(message);