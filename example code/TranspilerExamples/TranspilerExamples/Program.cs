using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;

namespace TranspilerExamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Transpiling!");

            var p = new PlayerClass();
            p.BananaCheck();
            p.BanananaB();
            p.LoopingLoop();

            Console.WriteLine(p.Balling());

            new Harmony("AAAAA").PatchAll();

            p.BananaCheck();
            p.BanananaB();
            p.LoopingLoop();

            Console.WriteLine(p.Balling());

            Console.ReadLine();
        }


        static float AddNumbers(int a, float b)
        {
            float sum = a + b;
            int sumInt = a + (int)b;

            float product = sum * sumInt;

            if (product >= 0)
            {
                return sum;
            }
            else if (product < 0 && sumInt == 0)
            {
                return 0f;
            }
            return product;
        }



        void CallMethods(int[] array)
        {
            int size = array.Length;

            AddNumbers(10, size);
            Console.WriteLine(array.Length);

            LoopFor(array);
            LoopWhile(array);
        }

        void LoopFor(int[] array)
        {
            int size = array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                size--;
            }
        }
        void LoopWhile(int[] array)
        {
            int size = array.Length;
            while (size > 0)
            {
                size--;
            }
        }
    }
    [HarmonyPatch]
    class Transpilers
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerClass), nameof(PlayerClass.Balling))]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
            .MatchForward(true,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerClass), "PlayerClass.Balling")),
                    new CodeMatch(OpCodes.Stloc_0)
                )
                .Advance(1) //Advance to after the assignment
                .CreateLabel(out Label labelAfterAssignment) //Creates a label which exits the loop
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    CodeInstruction.CallClosure<Func<string, bool>>((ball) =>
                    {
                        return ball == "not ball";
                    }),
                    new CodeInstruction(OpCodes.Brfalse, labelAfterAssignment), //Will not return if  ball != "not ball"
                    
                    new CodeInstruction(OpCodes.Ldc_I4, 101), //The return inside the if in your desired method
                    new CodeInstruction(OpCodes.Ret)
                )
             .InstructionEnumeration();
        }
    }
}
public class PlayerClass
{
    int bananas = 1000;

    public void BanananaB()
    {
        Console.WriteLine("You have {0} bananas", bananas);
        Console.WriteLine("You still have {0} bananas", bananas);
    }

    public void BananaCheck()
    {
        if (bananas >= 10)
        {
            ExplodePlayer(10);
        }

        Console.WriteLine("I love my {0} bananas!", 10);

        int ten = 10;

        Console.WriteLine("This is a really cool number! {0}", ten + 1);

        Console.WriteLine("Good thing that ten is still {0}", ten);
    }

    public void LoopingLoop() 
    {
        int number = 0;
        for(int i = 0; i <= 1000; i++) 
        {
            number = i;
        }

        if(number == 1000)
        {
            Console.WriteLine("The loop passed through all the values");
        }
    }
    string IsBall() 
    {
        return "not ball";
    }
    public int Balling()
    {
        string ball = IsBall();

        int lenght = ball.Length;
        lenght += ball[0];

        return lenght;
    }


    void ExplodePlayer(int i)
    {
        Console.WriteLine("Booom! {0}", i);
    }
}
