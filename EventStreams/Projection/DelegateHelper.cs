using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventStreams.Projection {
    internal static class DelegateHelper {
        /// <summary>
        /// Creates a delegate of a specified type that represents a method which can be executed on an instance passed as parameter.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// The type for the delegate.
        /// This delegate needs at least one (first) type parameter denoting the type of the instance  which will be passed.
        /// </typeparam>
        /// <param name="method">The MethodInfo describing the method of the instance type.</param>
        public static TDelegate CreateOpenInstanceDelegate<TDelegate>(MethodInfo method) where TDelegate : class {
            var delegateMethodInfo = typeof(TDelegate).GetMethod("Invoke");
            var delegateParameters = delegateMethodInfo.GetParameters();

            // Convert instance type when necessary.
            var delegateInstanceType = delegateParameters.Select(p => p.ParameterType).First();
            var methodInstanceType = method.DeclaringType;
            var instance = Expression.Parameter(delegateInstanceType);
            var convertedInstance = Expression.Convert(instance, methodInstanceType);

            // Create delegate original and converted arguments.
            var delegateTypes = delegateParameters.Select(d => d.ParameterType).Skip(1);
            var methodTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();

            // Prepare delegate parameter expressions.
            var sourceDelegateParameterExpressions = delegateTypes.Select(Expression.Parameter).ToArray();
            var targetDelegateParameterExpressions = (IEnumerable<Expression>)new Expression[] {
                // Wrap the IStreamedEvent parameter in a cast to the concrete type.
		        Expression.Convert(sourceDelegateParameterExpressions[0], methodTypes[0]),
                // Pass-thru the Boolean parameter as it is.
		        sourceDelegateParameterExpressions[1]
		    };

            // Create method call.
            var methodCall = Expression.Call(
                convertedInstance,
                method,
                targetDelegateParameterExpressions);

            // Build a lambda and compile it into a delegate.
            return Expression
                .Lambda<TDelegate>(
                    methodCall,
                    new[] { instance }.Concat(sourceDelegateParameterExpressions))
                .Compile();
        }
    }
}
