using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventStreams.Projection {
    using Core.Domain;

    internal sealed class AggregateRootActivatorCache<TAggregateRoot> where TAggregateRoot : class, new() {

        private delegate object ObjectActivator(params object[] args);

        private static Func<Guid, TAggregateRoot> _cachedCtor;

        public Func<Guid, TAggregateRoot> Activator() {
            return _cachedCtor ?? (_cachedCtor = GetActivator());
        }

        private Func<Guid, TAggregateRoot> GetActivator() {
            var arCtorInfo =
                typeof (TAggregateRoot)
                    .GetConstructors()
                    .Where(ci => ci.GetParameters().Length == 1)
                    .SingleOrDefault(ci => ci.GetParameters().SingleOrDefault(pi => pi.ParameterType.GetGenericTypeDefinition() == typeof (Memento<>)) != null);

            if (arCtorInfo == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The aggregate root type '{0}' does not define a constructor that accepts a '{1}' type.",
                        typeof (TAggregateRoot), typeof (Memento<>)));

            var memCtorInfo =
                arCtorInfo
                    .GetParameters()
                    .First()
                    .ParameterType
                    .GetConstructor(new[] { typeof(Guid) });

            return arId => {
                var mementoActivator = GetCompiledActivator(memCtorInfo);
                var aggregateRootActivator = GetCompiledActivator(arCtorInfo);

                var memento = mementoActivator(arId);
                return (TAggregateRoot)aggregateRootActivator(memento);
            };
        }

        private static ObjectActivator GetCompiledActivator(ConstructorInfo ctor) {
            // Create a single param of type object[].
            var param = Expression.Parameter(typeof(object[]), "args");

            //
            var paramsInfo = ctor.GetParameters();
            var argsExp = new Expression[paramsInfo.Length];

            // Pick each arg from the params array 
            // and create a typed expression of them.
            for (var i = 0; i < paramsInfo.Length; i++) {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                var paramAccessorExp = Expression.ArrayIndex(param, index);
                var paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // Make a NewExpression that calls the
            // ctor with the args we just created.
            var newExp = Expression.New(ctor, argsExp);

            // Create a lambda with the New Expression
            // as body and our param object[] as arg.
            var lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            // Compile it!
            return (ObjectActivator)lambda.Compile();
        }
    }
}
