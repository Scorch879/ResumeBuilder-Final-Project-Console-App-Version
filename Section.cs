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
                        Console.WriteLine($"{fieldName} can't be empty. Please enter again!");
                        Console.Write($"Enter {fieldName}: ");
                    }
                } while (string.IsNullOrEmpty(input));

                return input;
            }
        }
}