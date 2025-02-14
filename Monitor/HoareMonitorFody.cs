using MethodDecorator.Fody.Interfaces;
using System;
using System.Reflection;
using System.Threading;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HoareMonitorFody : Attribute, IMethodDecorator
{
    public void Init(object instance, MethodBase method, object[] args) { }

    public void OnEntry()
    {
        Monitor.Enter(this);
    }

    public void OnExit()
    {
        Monitor.Exit(this);
    }

    public void OnException(Exception exception)
    {
        Monitor.Exit(this);
    }
}
