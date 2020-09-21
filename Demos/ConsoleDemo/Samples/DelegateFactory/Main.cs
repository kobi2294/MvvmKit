using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.DelegateFactory
{
    public static class Main
    {

        public static void Run()
        {
            var sig1 = Signature.Of<Func<Person, int, string>>();
            var sig2 = Signature.Of<Action<int, double, string>>();
            var sig3 = Signature.Of<Func<Person, int, string>>();

            var p1 = new Person { Name = "Kobi" };
            var p2 = new Person { Name = "Daniel" };


            var mi = typeof(Person).GetMethod("CalcAge");
            var mi2 = typeof(Person).GetMethod("CreateFromName");
            var ci = typeof(Person).GetConstructor(new[] { typeof(string), typeof(string) });

            var openDel = mi.CompileTo<Func<object, string, object>>();
            var closeDel = mi.CompileTo<Func<string, object>>(p1);
            var staticDel = mi2.CompileTo<Func<object, object>>("Kobi");
            var arrayDel = mi2.CompileToArrayFunc<Person>();
            var ctor = ci.CompileTo<Func<string, string, Person>>();
            var ctor2 = ci.CompileTo<Func<Person>>("John", "Lennon");
            var ctor3 = ci.CompileToArrayFunc<Person>();

            var customCtor = ci.CompileTo<Func<string, string, Person>>(prms => prms[1], prms => prms[0]);

            var res1 = openDel(p2, "Hello");
            Console.WriteLine($"Expected result: 11, actual result: {res1}");

            var res2 = closeDel("Hello");
            Console.WriteLine($"Expected result: 9, actual result: {res2}");

            var res3 = staticDel("Hari");
            Console.WriteLine($"Expected result: 'Person: Kobi Hari', actual result: {res3}");

            var res4 = arrayDel(new[] { "John", "Smith" });
            Console.WriteLine($"Expected result: 'Person: John Smith', actual result: {res4}");

            var res5 = ctor("Jane", "Smith");
            Console.WriteLine($"Expected result: 'Person: Jane Smith', actual result: {res5}");

            var res6 = ctor2();
            Console.WriteLine($"Expected result: 'Person: John Lennon', actual result: {res6}");

            var res7 = ctor3(new[] { "Ronni", "Hari" });
            Console.WriteLine($"Expected result: 'Person: Ronni Hari', actual result: {res7}");

            var res8 = ctor3(new[] { "Elad", "Perltzweig" });
            Console.WriteLine($"Expected result: 'Person: Perltzweig Elad', actual result: {res8}");


            var imsh = new ImmutableShape(42, "John Smith", 12.5, new Person("John", "Smith"));
            var imshctor = typeof(ImmutableShape).GetConstructors().First();

            var changeA = imshctor.CompileCopyConstructor<object, object>(typeof(ImmutableShape).GetProperty("A"));
            var changeB = imshctor.CompileCopyConstructor<object, object>(typeof(ImmutableShape).GetProperty("B"));

            var ca = changeA(imsh, 50);
            var cb = changeB(imsh, "Mister Jones");

            var nameProp = typeof(Person).GetProperty("Name");
            var personNameGetter = nameProp.CompileGetter<object, object>();
            var personNameSetter = nameProp.CompileSetter<object, object>();

            var oldName = personNameGetter(p1);
            personNameSetter(p1, "Yoyo Ma");
            var newName = personNameGetter(p1);

            var fi = typeof(Person).GetField("_secret", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldGetter = fi.CompileGetter<object, object>();
            var fieldSetter = fi.CompileSetter<object, object>();

            var oldSecret = fieldGetter(p1);
            fieldSetter(p1, "Covid-19");
            var newSecret = fieldGetter(p1);

            Console.WriteLine(p1);

            //var openCasted = CreateOpenDelegate<Func<Person, string, object>>(mi);
            //var res3 = openCasted(p2, "Hello");
            //Console.WriteLine($"Expected result: 11, actualt result: {res3}");
        }

        public static DelegateType CreateOpenDelegate<DelegateType>(MethodInfo mi)
        {
            throw new NotImplementedException();
        }
    }
}
