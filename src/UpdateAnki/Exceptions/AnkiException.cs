namespace UpdateAnki.Exceptions;

public sealed class AnkiException(string message) : Exception(message);
