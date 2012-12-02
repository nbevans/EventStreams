using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventStreams.Projection {
    internal sealed class ObjectActivatorCache<TModel> where TModel : class {

        private delegate object ObjectActivator(params object[] args);

        private const string MementoParamName = "memento";
        private const string StateParamName = "state";

        private static Func<Guid, TModel> _cachedCtor;

        public Func<Guid, TModel> Activator() {
            return _cachedCtor ?? (_cachedCtor = GetActivator());
        }

        private Func<Guid, TModel> GetActivator() {
            // Locate the appropriate TModel constructor.
            var modelCtorInfo =
                typeof(TModel)
                    .GetConstructors()
                    .Where(ci => ci.GetParameters().Length == 1)
                    .SingleOrDefault(ci => ci.GetParameters().SingleOrDefault(TestMementoParameterInfo) != null);

            if (modelCtorInfo == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The '{0}' model type does not define a constructor that accepts a memento or state object. " +
                        "Ensure that your type defines a single-parameter constructor where the parameter is named either '{1}' or '{2}'.",
                        typeof(TModel), MementoParamName, StateParamName));

            // Locate the memento constructor.
            var mementoCtorInfo =
                modelCtorInfo
                    .GetParameters()
                    .Single()
                    .ParameterType
                    .GetConstructor(new[] { typeof(Guid) });

            if (mementoCtorInfo == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The '{0}' model type uses a type of memento/state which does not define a constructor that accepts a mandatory identity value." +
                        "Ensure that your memento or state type defines a single-parameter constructor where the parameter is of '{1}' type.",
                        typeof (TModel), typeof (Guid)));

            return identity => {
                var mementoActivator = GetCompiledActivator(mementoCtorInfo);
                var modelActivator = GetCompiledActivator(modelCtorInfo);

                var memento = mementoActivator(identity);
                return (TModel)modelActivator(memento);
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

        private static bool TestMementoParameterInfo(ParameterInfo pi) {
            return pi.Name.Equals(MementoParamName, StringComparison.OrdinalIgnoreCase) ||
                   pi.Name.Equals(StateParamName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
