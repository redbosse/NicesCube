using System;

public static class TypeVariable
{
    public static Type GetType<T>(T parameter)
    {
        return typeof(T);
    }

    public static string GetTypeName<T>(T parameter)
    {
        return typeof(T).Name;
    }
}