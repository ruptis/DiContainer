namespace DiContainer;

public class DiContainerException(Type InvalidType, string message) : Exception(message);
