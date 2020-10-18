using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Randoms
    {
        #region Data

        private static readonly string[] _words = new string[]
        {
            "jet","only","everyone","popular","wagon","being","depth","explanation","discover","attached",
            "many","three","near","curious","desk","angle","which","signal","press","walk",
            "engineer","pay","it","what","light","globe","independent","food","they","stronger",
            "string","society","task","yard","alive","week","baseball","potatoes","pure","blew",
            "today","whatever","tin","broad","shall","day","verb","tin","say","strike",
            "certain","complete","neighbor","read","market","thy","remove","problem","pile","spell",
            "home","crowd","coast","support","hit","window","some","father","hospital","play",
            "grabbed","off","height","firm","water","ill","unit","aside","depend","exactly",
            "activity","parts","idea","aid","replied","leaf","copy","alive","seen","mine",
            "earlier","everything","cotton","trap","instead","form","usually","while","graph","victory",
            "break","find","load","she","got","obtain","symbol","about","electricity","ran",
            "prove","police","opportunity","harbor","softly","torn","speak","growth","chamber","son",
            "member","review","indeed","dirty","serve","sport","whom","divide","again","bent",
            "bare","over","fix","congress","learn","look","generally","against","plant","shout",
            "term","frequently","age","doll","face","highway","halfway","least","blue","mile",
            "danger","bank","simplest","basic","sister","powerful","pitch","forest","piano","parts",
            "man","dried","farm","escape","observe","dozen","spell","lower","equator","same",
            "pictured","store","front","neighbor","half","usual","drop","force","bigger","bush",
            "girl","finger","leaving","page","tonight","with","supper","camp","furniture","mice",
            "badly","agree","growth","attention","master","please","collect","rate","join","glass",
            "coast","fully","stop","lead","flew","height","seeing","eaten","lying","company",
            "another","again","stove","greatest","expression","somewhere","fun","path","tune","completely",
            "wire","thus","main","sunlight","region","effect","surprise","imagine","curve","wind",
            "divide","arrive","both","game","has","in","distance","desert","difference","silence",
            "thought","window","sort","layers","race","repeat","quietly","saved","indicate","look",
            "price","ship","agree","popular","animal","label","slabs","mixture","long","met",
            "individual","do","girl","smallest","complex","event","whispered","deer","rod","slope",
            "oxygen","yellow","real","settlers","happy","courage","lift","society","hall","automobile",
            "two","temperature","cattle","traffic","dawn","rock","simplest","creature","usual","pretty",
            "lunch","quite","ear","whole","involved","water","possibly","art","second","window",
            "foreign","worried","means","hollow","identity","under","owner","swept","previous","temperature"
        };

        private static readonly string[] _firstNames = new string[]
        {
            "Kendra", "Blanca", "Alberta", "Gilbert", "Rebekah", "Savage", "Cathryn", "Shelton", "Beth", "Rice", "Suarez", "Haley",
            "Jennifer", "Berger", "Edith", "Deborah", "Graham", "Lizzie", "Mayer", "Noelle", "Chasity", "Minerva", "Alston", "Burks",
            "Sargent", "Lucy", "Madeline", "Wheeler", "Kirk", "Oliver", "Koch", "Allyson", "Manuela", "Haynes", "Pratt", "Kimberly",
            "Esperanza", "Knowles", "Queen", "Evangelina", "Hayes", "Doyle", "Tia", "Rochelle", "Rowena", "Peters", "Tracy", "Lela",
            "Warner", "Kelly", "Russo", "Maggie", "Estrada", "Hobbs", "Gray", "Franco", "Alba", "Joanne", "Patrick", "Sylvia",
            "Nelda", "Kathryn", "Thompson", "Leanne", "Dixie", "Avila", "Ramona", "Mack", "Hart", "Cervantes", "Hurst", "Angie",
            "Ballard", "Mai", "Everett", "Molina", "Edwina", "Loretta", "Richards", "Cardenas", "Tommie", "Horton", "Janelle",
            "Frank", "Janet", "Barr", "Baird", "Gibbs", "Mayo", "Hope", "Shawna", "Eleanor", "Meyer", "Louella", "Ester", "Alyce",
            "Staci", "Roseann", "Eileen", "Mathews"
        };

        private static readonly string[] _lastNames = new string[]
        {
            "Shannon","Saunders","Manning","Mcmillan","Witt","Vargas","Norman","Evans","Kerr","Hudson","Beck","Barlow","Salas",
            "Colon","Vang","Wood","Nelson","Charles","Frank","Hale","Jenkins","Rosales","Santana","Clay","Oneil","Santiago","Harvey",
            "Dixon","Mcgowan","Randolph","Payne","Osborne","Browning","Marquez","Ray","Riddle","Reynolds","Wagner","Fletcher","Mckay",
            "Mckenzie","Fitzgerald","Raymond","Mcguire","Mendoza","Perez","Combs","Mullins","Merrill","Brooks","Cortez","Johns",
            "Little","Oneal","Martin","Wyatt","Adkins","George","Harmon","Austin","Stafford","West","Hull","Dunlap","Rosa","Benson",
            "Horne","Cabrera","Rodgers","Smith","Sloan","Buckley","Castillo","Thomas","Compton","Shaffer","Tillman","Juarez","Fowler",
            "Price","Joyce","Simon","Cote","Sanchez","Hubbard","Kelley","Mathis","Wilder","Washington","Douglas","Rocha","Guy",
            "Hoffman","Larsen","Ramirez","Christensen","Preston","Dodson","Pearson","Finley"
        };

        private static readonly string[] _links = new string[]
        {
            "http://www.google.com",
            "http://www.walla.co.il",
            "http://www.one.co.il",
            "http://www.yahoo.com"
        };


        #endregion

        private Random _rand;
        private object _lock = new object();

        public static Randoms Default { get; } = new Randoms();


        public Randoms()
        {
            if (Default == null)
            {
                _rand = new Random((int)DateTime.Now.Ticks);
            }
            else
            {
                _rand = new Random(Randoms.Default.Next(0, int.MaxValue));
            }

        }


        public T From<T>(params T[] options)
        {
            lock (_lock)
            {
                return options[_rand.Next(options.Length)];
            }
        }

        public T OneOf<T>(IEnumerable<T> possibleOptions)
        {
            lock (_lock)
            {
                var options = possibleOptions.ToArray();
                var index = _rand.Next(options.Length);
                var res = options[index];
                return res;
            }
        }

        public T OneOfOptions<T>(params T[] options)
        {
            lock (_lock)
            {
                var index = _rand.Next(options.Length);
                var res = options[index];
                return res;
            }
        }

        public int Next(int min, int max)
        {
            lock (_lock)
            {
                return _rand.Next(min, max);
            }
        }

        public string NextWords(int wordsCount)
        {
            return String.Join(" ",
                Enumerable.Range(0, wordsCount)
                    .Select(_ => OneOfOptions(_words)));
        }

        public string NextFirstName()
        {
            return OneOfOptions(_firstNames);
        }

        public string NextLastName()
        {
            return OneOfOptions(_lastNames);
        }

        public string NextFullName()
        {
            return $"{NextFirstName()} {NextLastName()}";
        }

        public IEnumerable<int> NextRange(int minCount, int maxCount, int start = 0)
        {
            return Enumerable.Range(start, Next(minCount, maxCount));
        }

        public string NextWords(int minCount, int maxCount)
        {
            return NextWords(Next(minCount, maxCount));
        }

        public string NextImageUrl()
        {
            var width = Next(2, 9) * 100;
            var height = Next(2, 6) * 100;
            var id = Next(1, 400);

            //return $"https://i.picsum.photos/id/{id}/{width}/{height}.jpg";
            //return $"https://loremflickr.com/{width}/{height}/hardware?lock={id}";
            return $"https://placeimg.com/{width}/{height}/{id}";
        }

        public string NextLinkUrl()
        {
            return OneOfOptions(_links);
        }

        public char _template(char template)
        {
            lock (_lock)
            {
                if (template == '#')
                    return (char)('0' + _rand.Next(10));

                if (template == 'a')
                    return (char)('a' + _rand.Next(26));

                if (template == 'A')
                    return (char)('A' + _rand.Next(26));

                return template;
            }
        }

        public string FromTemplate(string template)
        {
            return new string(template.Select(c => _template(c)).ToArray());
        }

        public T FromEnum<T>()
        {
            if (!typeof(T).IsEnum) throw new InvalidOperationException($"{typeof(T).Name} is not an enum");
            var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            var res = OneOfOptions(values);
            return res;
        }

        public bool Toss(double trueRate = 0.5)
        {
            lock (_lock)
            {
                return _rand.NextDouble() < trueRate;
            }
        }
    }
}
