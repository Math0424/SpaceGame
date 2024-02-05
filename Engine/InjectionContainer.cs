using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal class InjectionContainer
    {
        Dictionary<Type, object> resolver = new Dictionary<Type, object>();

        public InjectionContainer()
        {

        }

        public T Resolve<T>()
        {
            RegisterType<T, T>();
            return (T)resolver[typeof(T)];
        }

        public void RegisterType<T, U>()
        {
            if (resolver.ContainsKey(typeof(T)))
                throw new Exception($"Type {typeof(T)} already found in resolver!");

            object instance = FormatterServices.GetUninitializedObject(typeof(T));
            var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var elm = constructors.Where(c => c.GetParameters().Length != 0);
            Console.WriteLine($"Creating {typeof(T)}");
            if (elm.Count() != 0)
            {
                var p = elm.First();
                Console.WriteLine($" - constuctor with {p.GetParameters().Length} args");

                object[] objects = new object[p.GetParameters().Length];
                for (int i = 0; i < p.GetParameters().Length; i++)
                {
                    Type bas = p.GetParameters()[i].ParameterType;
                    foreach(var x in resolver.Values)
                    {
                        if (bas.IsAssignableFrom(x.GetType()))
                        {
                            objects[i] = x;
                        }
                    }
                    if (objects[i] == null)
                        throw new Exception($"Cannot resolve class {bas}!");
                }
                p.Invoke(instance, objects);
            } 
            else
            {
                Console.WriteLine($" - null constructor");
                typeof(T).GetConstructor(Type.EmptyTypes).Invoke(instance, null);
            }
            resolver[typeof(T)] = instance;
        }

        public void RegisterClass(object obj)
        {
            if (resolver.ContainsKey(obj.GetType()))
                throw new Exception("Type already found in resolver!");
            resolver[obj.GetType()] = obj;
        }


    }
}
