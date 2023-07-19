using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

namespace CommerceApiDem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<CommerceApiDem.Models.Product> Product { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.ProductCategory> ProductCategory { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.StateLocation> StateLocation { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.Order> Order { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.OrderProduct> OrderProduct { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.OrderStatus> OrderStatus { get; set; } = default!;
        public DbSet<CommerceApiDem.Models.OrderHistory> OrderHistory { get; set; } = default!;




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CommerceApiDem.Models.OrderStatus>().ToTable("OrderStatus");

            /*
            #region product category
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, Title = "Sports" },
                new ProductCategory { Id = 2, Title = "Household" },
                new ProductCategory { Id = 3, Title = "Technology" },
                new ProductCategory { Id = 4, Title = "Fashion" }
            );
            
            #endregion


            #region product 
            
            modelBuilder.Entity<Product>().HasData(
                // Sports
                new Product { Id = 1, Title = "Air Jordan Sneaker", Brand = "Nike", Description = "Original Air Jordan", Price = 119.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S001" }
               
                new Product { Id = 2, Title = "Predator", Brand = "Adidas", Description = "Soccer shoe", Price = 79.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S002" },
                new Product { Id = 3, Title = "Tiger Woods Shirt", Brand = "Nike", Description = "Tiger Woods final round shirt", Price = 49.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S003" },
                new Product { Id = 4, Title = "Workout Top", Brand = "Under Armour", Description = "Basic workout shirt", Price = 39.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S004" },
                new Product { Id = 5, Title = "Workout Short", Brand = "Under Armour", Description = "Basic workout short", Price = 39.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S005" },
                new Product { Id = 6, Title = "Attack Helmet", Brand = "Giro", Description = "Giro Attack aerodymic cycling helmet", Price = 99.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S006" },
                new Product { Id = 7, Title = "X-40 Pickleball Balls", Brand = "Franklin", Description = "Franklin Sports Outdoor Pickleballs - X-40 Pickleball Balls - USA Pickleball (USAPA) Approved - Official US Open Ball - 3 Packs, 12 Packs, 36 Pickleball Buckets, 100 + 400 Bulk Packs of Pickleballs", Price = 26.75M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S007" },
                new Product { Id = 8, Title = "Pickleball Paddle", Brand = "JOOLA", Description = "JOOLA Ben Johns Hyperion CFS Pickleball Paddle - Carbon Surface with High Grit & Spin, Elongated Handle, USAPA Approved 2022 Ben Johns Paddle", Price = 189.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S008" },
                new Product { Id = 9, Title = "Pickleball Net", Brand = "flybold", Description = "flybold Pickleball Net |Portable Set with/without 4 Paddles| Net Regulation Size Equipment Lightweight Sturdy Interlocking Metal Posts with Carrying Bag for Indoor Outdoor Game| Full Court Size - 22ft", Price = 99.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S009" },
                new Product { Id = 10, Title = "Camping Tent", Brand = "Coleman", Description = "Coleman Sundome Camping Tent, 2/3/4/6 Person Dome Tent with Easy Setup, Included Rainfly and WeatherTec Floor to Block Out Water, 2 Windows and 1 Ground Vent for Air Flow with Charging E-Port Flap", Price = 127.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S010" },
                new Product { Id = 11, Title = "Watersports Tube", Brand = "AIRHEAD", Description = "AIRHEAD G-Force, 1-4 Rider Towable Tube for Boating, Multiple Size Options Available", Price = 142.30M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S011" },
                new Product { Id = 12, Title = "Stand Up Paddle Board", Brand = "Aqua Plus", Description = "Aqua Plus 6inches Thick Inflatable SUP for All Skill Levels Stand Up Paddle Board,Paddle,Double Action Pump,ISUP Travel Backpack, Leash,Shoulder Strap,Youth,Adult Inflatable Paddle Board", Price = 169.99M, AvailableQty = 30, ProductCategoryId = 1, ModelNumber = "S012" }
                
                // Household
                new Product { Id = 13, Title = "Rivet Aiden Mid-Century Modern Tufted Velvet Loveseat Sofa", Brand = "Rivet", Description = "This sleek, mid-Century inspired loveseat is designed to impress. ", Price = 739.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H001" },
                new Product { Id = 14, Title = "HERCULES Lesley Series Contemporary Black LeatherSoft Sofa with Encasing Frame", Brand = "Flash Furniture", Description = "You're great at what you do and your company has grown so make sure you convey that to clients when you choose your reception furniture.", Price = 1257.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H002" },
                new Product { Id = 15, Title = "Premium Microfiber Cleaning Cloth", Brand = "OVWO", Description = "80% Polyester/ 20% Polyamide", Price = 9.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H003" },
                new Product { Id = 16, Title = "Bathroom Cleaner Spray", Brand = "Lysol", Description = "KILLS 99.9% OF BATHROOM VIRUSES AND BACTERIA", Price = 3.97M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H004" },
                new Product { Id = 17, Title = "Linen White Bath Towels 4-Pack", Brand = "Hammam", Description = " 4-Piece super soft and absorbent Turkish cotton bath towels.", Price = 33.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H005" },
                new Product { Id = 18, Title = "Sure-Crisp Air Fryer Countertop Toaster Oven", Brand = "Hamilton Beach", Description = "The Sure-Crisp air fry convection function circulates air around food as it cooks", Price = 69.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H005" },
                new Product { Id = 19, Title = "Floor Sweeper", Brand = "Bissell", Description = "Bissell Easy Sweep Compact Carpet & Floor Sweeper", Price = 20.59M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H007" },
                new Product { Id = 20, Title = "Portable Space Heater", Brand = "BLACK+DECKER", Description = "BLACK+DECKER Portable Space Heater, 1500W Room Space Heater with Carry Handle for Easy Transport", Price = 24.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H008" },
                new Product { Id = 21, Title = "Electric Infrared Quartz Fireplace Stove", Brand = "Duraflame", Description = "Duraflame Electric Infrared Quartz Fireplace Stove with 3D Flame Effect, Black", Price = 219.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H009" },
                new Product { Id = 22, Title = "Evaporative Air Cooler", Brand = "Arctic Air", Description = "Arctic Air Pure Chill 2.0 Evaporative Air Cooler by Ontel - Powerful, Quiet, Lightweight and Portable Space Cooler with Hydro-Chill Technology For Bedroom, Office, Living", Price = 31.95M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H010" },
                new Product { Id = 23, Title = "Window Air Conditioner", Brand = "LG", Description = "LG 6,000 BTU Window Conditioner, Cools 250 Sq.Ft. (10' x 25' Room Size), Quiet Operation, Electronic Control with Remote, 2 Cooling & Fan Speeds, 2-Way Air Deflection, Auto Restart, 115V, White", Price = 189.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H011" },
                new Product { Id = 24, Title = "Humidifier", Brand = "Frida Baby", Description = "Frida Baby 3-in-1 Humidifier with Diffuser and Nightlight, White", Price = 44.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H012" },
                new Product { Id = 25, Title = "Air Purifier", Brand = "LEVOIT", Description = "LEVOIT Air Purifiers for Bedroom Home, HEPA Filter Cleaner with Fragrance Sponge for Better Sleep, Filters Smoke, Allergies, Pet Dander, Odor, Dust, Office, Desktop, Portable, Core Mini, White", Price = 49.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H013" },
                new Product { Id = 26, Title = "Area Rug", Brand = "Artistic Weavers", Description = "Artistic Weavers Odelia Vintage Bohemian Area Rug,Aqua", Price = 99.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H014" },
                new Product { Id = 27, Title = "3-Tier Shoe Rack", Brand = "Simple Houseware", Description = "Simple Houseware 3-Tier Shoe Rack Storage Organizer 12-Pair / 20-Pair, Bronze", Price = 19.99M, AvailableQty = 30, ProductCategoryId = 2, ModelNumber = "H015" }
                
                // Technology
                new Product { Id = 28, Title = "Inspiron 3511 Premium Laptop", Brand = "Dell", Description = "Intel Core i5-1035G1 Quad-Core Processor (4Cores, 6MB Intel Smart Cache, 8 Threads", Price = 679.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T001" },
                new Product { Id = 29, Title = "XPS 17 9720 Laptop", Brand = "Dell", Description = "12th Generation Intel Core i7-12700H (24MB Cache, up to 4.7 GHz, 14 cores)", Price = 2499.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T002" },
                new Product { Id = 30, Title = "iPhone 13 Pro", Brand = "Apple", Description = "6.1-inch Super Retina XDR display with ProMotion for a faster, more responsive feel", Price = 999.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T003" },
                new Product { Id = 31, Title = "iPhone 13 Mini", Brand = "Apple", Description = "5.4-inch Super Retina XDR display", Price = 729.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T004" },
                new Product { Id = 32, Title = "65-Inch Class UQ9000", Brand = "LG", Description = "Everything you need to bring your favorite content to life with the power of 4K and the extras you crave.", Price = 576.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T005" },
                new Product { Id = 33, Title = "iPad Air", Brand = "Apple", Description = "10.9-inch, Wi-Fi + Cellular, 64GB", Price = 749.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T006" },
                new Product { Id = 34, Title = "Wireless Speakers", Brand = "Roku", Description = "Roku Wireless Speakers (for Roku Streambars or Roku TV)", Price = 149.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T007" },
                new Product { Id = 35, Title = "Headphones", Brand = "Sennheiser", Description = "Sennheiser HD 400S Closed Back, Around Ear Headphone with One-Button Smart Remote on Detachable Cable", Price = 49.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T008" },
                new Product { Id = 36, Title = "Projector ", Brand = "Elephas", Description = "Mini Projector for iPhone, ELEPHAS 2023 Upgraded 1080P HD Projector, 8000L Portable Projector with Tripod and Carry Bag, Movie Projector Compatible with Android/iOS/Windows/TV Stick/HDMI/USB", Price = 99.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T009" },
                new Product { Id = 37, Title = "AirTag", Brand = "Apple", Description = "Apple AirTag", Price = 27.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T010" },
                new Product { Id = 38, Title = "Inspiron 3511 Premium Laptop", Brand = "Dell", Description = "Intel Core i5-1035G1 Quad-Core Processor (4Cores, 6MB Intel Smart Cache, 8 Threads", Price = 679.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T001" },
                new Product { Id = 39, Title = "Headphones", Brand = "Dyson", Description = "Dyson Zone™ headphones", Price = 949.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T011" },
                new Product { Id = 40, Title = "PlayStation 5 Console ", Brand = "Sony", Description = "PlayStation 5 Console (PS5)", Price = 499.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T012" },
                new Product { Id = 41, Title = "Radar Detector", Brand = "Cobra", Description = "Cobra RAD 480i Laser Radar Detector – Long Range Detection, Bluetooth, iRadar App, LaserEye Front and Rear Detection, Next Gen IVT Filtering, Black", Price = 149.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T013" },
                new Product { Id = 42, Title = "Dash Cam", Brand = "Rove", Description = "Rove R2-4K Dash Cam Built in WiFi GPS Car Dashboard Camera Recorder with UHD 2160P", Price = 119.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T014" },
                new Product { Id = 43, Title = "Watch Charger", Brand = "Nuinno", Description = "Watch Charger for Apple Watch Charger,USB C 20W Fast Charger Adapter 6.6FT", Price = 15.99M, AvailableQty = 30, ProductCategoryId = 3, ModelNumber = "T015" },

                // Fashion
                new Product { Id = 44, Title = "Mens Tilden Cap Oxford Shoe", Brand = "Clarks ", Description = "A classic captoe derby crafted from rich, full grain leather.", Price = 59.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F001" },
                new Product { Id = 45, Title = "Mens Cotrell Step Slip-On Loafer", Brand = "Clarks", Description = "Slip-on casual", Price = 54.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F002" },
                new Product { Id = 46, Title = "Mens Dress Shirt Regular Fit Poplin Solid", Brand = "Van Heusen", Description = "Fabric features enhanced wrinkle resistance for easy care at home.", Price = 24.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F003" },
                new Product { Id = 47, Title = "Mens Long Sleeve Dress Shirt", Brand = "Alberto Danelli", Description = "Our cotton blend dress shirt is the perfect fitting day to night closet must have. ", Price = 29.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F004" },
                new Product { Id = 48, Title = "Mens Classic Fit Easy Khaki", Brand = "Dockers", Description = "64% Cotton, 34% Polyester, 2% Elastane", Price = 39.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F005" },
                new Product { Id = 49, Title = "Mens American Chino Flat Front Straight Fit Pant", Brand = "Izod", Description = "100% Cotton", Price = 31.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F006" },
                new Product { Id = 50, Title = "Child Rain Boot", Brand = "Crocs", Description = "Crocs Unisex-Child Rain Boot", Price = 19.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F007" },
                new Product { Id = 51, Title = "Leggings ", Brand = "IUGA", Description = "IUGA Girls Athletic Leggings with Pockets Running Yoga Pants Girl's Workout Dance Leggings Tights for Girls High Waisted", Price = 17.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F008" },
                new Product { Id = 52, Title = "Skinny Jeans", Brand = "Childrens Place", Description = "The Childrens Place Girls' Super Skinny Jeans", Price = 14.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F009" },
                new Product { Id = 53, Title = "Jogger Pant", Brand = "adidas", Description = "adidas Boys' Iconic Tricot Jogger Pant", Price = 28.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F010" },
                new Product { Id = 54, Title = "Sunglasses", Brand = "Oakley", Description = "Oakley Men's OO9096 Fuel Cell Wrap Sunglasses", Price = 59.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F011" },
                new Product { Id = 55, Title = "Flip-Flop", Brand = "Reef", Description = "Reef Men's Phantoms Flip-Flop", Price = 22.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F012" },
                new Product { Id = 56, Title = "Men’s Swim Trunks", Brand = "Body Glove", Description = "Body Glove Men’s Swim Trunks – Stretch Fit Bathing Suit for Men", Price = 16.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F013" },
                new Product { Id = 57, Title = "Polo Shirt", Brand = "Nautica", Description = "Nautica Mens Classic Short Sleeve Solid Performance Deck Polo Shirt", Price = 26.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F014" },
                new Product { Id = 58, Title = "Cotton Boxers", Brand = "POLO", Description = "POLO Ralph Lauren Men's Classic Fit Cotton Woven Boxers 3-Pack", Price = 41.99M, AvailableQty = 30, ProductCategoryId = 4, ModelNumber = "F015" }
                
            );
            

            #endregion

            #region order status
            modelBuilder.Entity<Product>().HasData(
                new OrderStatus { Id = 1, Name = "Cart" },
                new OrderStatus { Id = 2, Name = "Processing" },
                new OrderStatus { Id = 3, Name = "Shipped" },
                new OrderStatus { Id = 4, Name = "Delivered" },
                new OrderStatus { Id = 5, Name = "Returned" }
            );
            #endregion

            #region states
            // US States
            modelBuilder.Entity<Product>().HasData(
                new StateLocation { Id = 1, Name = "Alabama", Abbreviation = "AL", TaxRate = 0.04M },
                new StateLocation { Id = 2, Name = "Alaska", Abbreviation = "AK", TaxRate = 0.05M },
                new StateLocation { Id = 3, Name = "Arizona", Abbreviation = "AZ", TaxRate = 0.06M },
                new StateLocation { Id = 4, Name = "Arkansas", Abbreviation = "AR", TaxRate = 0.07M },
                new StateLocation { Id = 5, Name = "California", Abbreviation = "CA", TaxRate = 0.08M },
                new StateLocation { Id = 6, Name = "Colorado", Abbreviation = "CO", TaxRate = 0.07M },
                new StateLocation { Id = 7, Name = "Connecticut", Abbreviation = "CT", TaxRate = 0.06M },
                new StateLocation { Id = 8, Name = "Delaware", Abbreviation = "DE", TaxRate = 0.05M },
                new StateLocation { Id = 9, Name = "Florida", Abbreviation = "FL", TaxRate = 0.04M },
                new StateLocation { Id = 10, Name = "Georgia", Abbreviation = "GA", TaxRate = 0.05M },
                new StateLocation { Id = 11, Name = "Hawaii", Abbreviation = "HI", TaxRate = 0.06M },
                new StateLocation { Id = 12, Name = "Idaho", Abbreviation = "ID", TaxRate = 0.07M },
                new StateLocation { Id = 13, Name = "Illinois", Abbreviation = "IL", TaxRate = 0.08M },
                new StateLocation { Id = 14, Name = "Indiana", Abbreviation = "IN", TaxRate = 0.07M },
                new StateLocation { Id = 15, Name = "Iowa", Abbreviation = "IA", TaxRate = 0.06M },
                new StateLocation { Id = 16, Name = "Kansas", Abbreviation = "KS", TaxRate = 0.05M },
                new StateLocation { Id = 17, Name = "Kentucky", Abbreviation = "KY", TaxRate = 0.04M },
                new StateLocation { Id = 18, Name = "Louisiana", Abbreviation = "LA", TaxRate = 0.05M },
                new StateLocation { Id = 19, Name = "Maine", Abbreviation = "ME", TaxRate = 0.06M },
                new StateLocation { Id = 10, Name = "Maryland", Abbreviation = "MD", TaxRate = 0.07M },
                new StateLocation { Id = 11, Name = "Massachusetts", Abbreviation = "MA", TaxRate = 0.08M },
                new StateLocation { Id = 12, Name = "Michigan", Abbreviation = "MI", TaxRate = 0.07M },
                new StateLocation { Id = 13, Name = "Minnesota", Abbreviation = "MN", TaxRate = 0.06M },
                new StateLocation { Id = 14, Name = "Mississippi", Abbreviation = "MS", TaxRate = 0.05M },
                new StateLocation { Id = 15, Name = "Missouri", Abbreviation = "MO", TaxRate = 0.04M },
                new StateLocation { Id = 16, Name = "Montana", Abbreviation = "MT", TaxRate = 0.05M },
                new StateLocation { Id = 17, Name = "Nebraska", Abbreviation = "NE", TaxRate = 0.06M },
                new StateLocation { Id = 18, Name = "Nevada", Abbreviation = "NV", TaxRate = 0.07M },
                new StateLocation { Id = 19, Name = "New Hampshire", Abbreviation = "NH", TaxRate = 0.08M },
                new StateLocation { Id = 20, Name = "New Jersey", Abbreviation = "NJ", TaxRate = 0.08M },
                new StateLocation { Id = 21, Name = "New Mexico", Abbreviation = "NM", TaxRate = 0.07M },
                new StateLocation { Id = 22, Name = "New York", Abbreviation = "NY", TaxRate = 0.06M },
                new StateLocation { Id = 23, Name = "North Carolina", Abbreviation = "NC", TaxRate = 0.05M },
                new StateLocation { Id = 24, Name = "North Dakota", Abbreviation = "ND", TaxRate = 0.04M },
                new StateLocation { Id = 25, Name = "Ohio", Abbreviation = "OH", TaxRate = 0.08M },
                new StateLocation { Id = 26, Name = "Oklahoma", Abbreviation = "OK", TaxRate = 0.04M },
                new StateLocation { Id = 27, Name = "Oregon", Abbreviation = "OR", TaxRate = 0.05M },
                new StateLocation { Id = 28, Name = "Pennsylvania", Abbreviation = "PA", TaxRate = 0.08M },
                new StateLocation { Id = 29, Name = "Rhode Island", Abbreviation = "RI", TaxRate = 0.07M },
                new StateLocation { Id = 30, Name = "South Carolina", Abbreviation = "SC", TaxRate = 0.04M },
                new StateLocation { Id = 31, Name = "South Dakota", Abbreviation = "SD", TaxRate = 0.04M },
                new StateLocation { Id = 32, Name = "Tennessee", Abbreviation = "TN", TaxRate = 0.04M },
                new StateLocation { Id = 33, Name = "Texas", Abbreviation = "TX", TaxRate = 0.0M },
                new StateLocation { Id = 34, Name = "Utah", Abbreviation = "UT", TaxRate = 0.05M },
                new StateLocation { Id = 35, Name = "Vermont", Abbreviation = "VT", TaxRate = 0.05M },
                new StateLocation { Id = 36, Name = "Virginia", Abbreviation = "VA", TaxRate = 0.06M },
                new StateLocation { Id = 37, Name = "Washington", Abbreviation = "WA", TaxRate = 0.04M },
                new StateLocation { Id = 38, Name = "West Virginia", Abbreviation = "WV", TaxRate = 0.03M },
                new StateLocation { Id = 39, Name = "Wisconsin", Abbreviation = "WI", TaxRate = 0.06M },
                new StateLocation { Id = 40, Name = "Wyoming", Abbreviation = "WY", TaxRate = 0.00M },
                new StateLocation { Id = 41, Name = "District of Columbia", Abbreviation = "DC", TaxRate = 0.06M },
                new StateLocation { Id = 42, Name = "Guam", Abbreviation = "GU", TaxRate = 0.06M },
                new StateLocation { Id = 43, Name = "Marshall Islands", Abbreviation = "MH", TaxRate = 0.06M },
                new StateLocation { Id = 44, Name = "Northern Mariana Island", Abbreviation = "MP", TaxRate = 0.06M },
                new StateLocation { Id = 45, Name = "Puerto Rico", Abbreviation = "PR", TaxRate = 0.06M },
                new StateLocation { Id = 46, Name = "Virgin Islands", Abbreviation = "VI", TaxRate = 0.06M }
                );

            #endregion
            */
        }
    }
}