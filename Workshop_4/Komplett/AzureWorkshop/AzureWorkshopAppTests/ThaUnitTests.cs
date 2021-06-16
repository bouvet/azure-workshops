using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AzureWorkshopAppTests
{
    public class ThaUnitTests
    {
        private ITestOutputHelper console;

        public ThaUnitTests(ITestOutputHelper output)
        {
            console = output;
        }

        [Fact]
        public void TestShouldPass()
        {
            const bool testShouldPass = true;

            Assert.True(testShouldPass);
        }

        [Fact]
        public void FizzBuzzTest()
        {
            console.WriteLine(FizzBuzzLinq(15));
            Assert.Equal(FizzBuzzIfs(1000).Trim(), FizzBuzzLinq(1000));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Enumerable.Range(1, 300).ToList().ForEach(n => FizzBuzzLinq(n * 10));
            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            Enumerable.Range(1, 300).ToList().ForEach(n => FizzBuzzIfs(n * 10).Trim());
            sw2.Stop();

            console.WriteLine($"SW linq: {sw.ElapsedTicks}\nSW ifs: {sw2.ElapsedTicks}");
            Assert.True(sw.ElapsedTicks > sw2.ElapsedTicks);
        }


        public string FizzBuzzLinq(int x) => 
            string.Join("\r\n", 
                Enumerable.Range(1, x).Select(n =>
                    (n % 5, n % 3) switch
                    {
                        (0, 0) => "FizzBuzz",
                        (_, 0) => "Fizz",
                        (0, _) => "Buzz",
                        (_, _) => $"{n}"
                    })
                );

        public string FizzBuzzIfs(int x)
        {
            var list = Enumerable.Range(1, x);
            var s = new StringBuilder();
            foreach(var n in list)
            {
                if (n % 3 == 0)
                {
                    if (n % 5 == 0)
                        s.AppendLine("FizzBuzz");
                    else
                        s.AppendLine("Fizz");
                }
                else if (n % 5 == 0)
                    s.AppendLine("Buzz");
                else
                    s.AppendLine($"{n}");
            }
            return s.ToString();
        }
    }
}
