using System;
using System.Linq;
using vault.commons;

namespace vault.exceptions;

public class VaultException : Exception
{
    public VaultException(string msg) : base(msg)
    {
        
    }
}

public class SpaceNotFoundException : VaultException
{
    public SpaceNotFoundException(string space) 
        : base($"Space '{space}' is not found.") { }
}

public class FieldNotFoundException : VaultException
{
    public FieldNotFoundException(string space, string app, string field) : 
        base($"Field '{field}' in '{space}/{app}' is not found.") { }
}

public class AppNotFoundException : VaultException
{
    public AppNotFoundException(string space, string app) : 
        base($"Field '{app}' in '{space}' space is not found.") { }
}

public class ValidationException : VaultException
{
    public ValidationException(string[] fields, string error) : base($"Fields '{fields.Join(',')}' {error}")
    {
        
    }
}