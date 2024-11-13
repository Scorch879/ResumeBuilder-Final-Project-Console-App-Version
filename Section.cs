namespace ResumeBuilderApp
{ 
        public abstract class Section
        {
            public abstract void CollectData();
            public abstract override string ToString();

            public virtual string GetInput(string fieldName)
            {
                string? input;
                do
                {
                    input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{fieldName} can't be empty. Please enter again!"); Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"Enter {fieldName}: "); Console.ResetColor();
                    }
                } while (string.IsNullOrEmpty(input));

                return input;
            }
        }
}