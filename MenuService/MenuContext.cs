using Microsoft.EntityFrameworkCore;
using MenuService.Model;

namespace MenuService{
    public class MenuContext : DbContext{
        public MenuContext(DbContextOptions<MenuContext> options) : base(options){}

        public DbSet<Menu> Menus {get; set;}

    }
}