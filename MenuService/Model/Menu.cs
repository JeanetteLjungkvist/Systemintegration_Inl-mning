namespace MenuService.Model{
    public class Menu{
        public Guid Id {get; set;}
        
        public string Name {get; set;}
        
        public string Category {get; set;}

        public string Allergen {get; set;}

        public int Price {get; set;}

        public bool Available {get; set;}

    }
}