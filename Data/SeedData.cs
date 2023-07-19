using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceApiDem.Data
{
    public static class SeedData
    {
        public static async void Initialize(ApplicationDbContext context)
        {
           
            #region product category
            if (!context.ProductCategory.Any())
            {
                var categories = new ProductCategory[]
                {
                    new ProductCategory { Title = "Sports"},
                    new ProductCategory { Title = "Household"},
                    new ProductCategory { Title = "Technology"},
                    new ProductCategory { Title = "Fashion" }
                };

                foreach (var item in categories)
                {
                    context.ProductCategory.Add(item);
                }
                context.SaveChanges();
            }
            #endregion

            #region product
            if (!context.Product.Any())
            {
                var products = new Product[]
                {
                    // Sports
                    new Product{ Title="Air Jordan Sneaker", Brand="Nike", Description="Original Air Jordan", Price=119.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S001"},
                    new Product{ Title="Predator", Brand="Adidas", Description="Soccer shoe", Price=79.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S002" },
                    new Product{ Title="Tiger Woods Shirt", Brand="Nike", Description="Tiger Woods final round shirt", Price=49.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S003" },
                    new Product{ Title="Workout Top", Brand="Under Armour", Description="Basic workout shirt", Price=39.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S004" },
                    new Product{ Title="Workout Short", Brand="Under Armour", Description="Basic workout short", Price=39.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S005" },
                    new Product{ Title="Attack Helmet", Brand="Giro", Description="Giro Attack aerodymic cycling helmet", Price=99.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S006" },
                    new Product{ Title="X-40 Pickleball Balls", Brand="Franklin", Description="Franklin Sports Outdoor Pickleballs - X-40 Pickleball Balls - USA Pickleball (USAPA) Approved - Official US Open Ball - 3 Packs, 12 Packs, 36 Pickleball Buckets, 100 + 400 Bulk Packs of Pickleballs", Price=26.75M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S007" },
                    new Product{ Title="Pickleball Paddle", Brand="JOOLA", Description="JOOLA Ben Johns Hyperion CFS Pickleball Paddle - Carbon Surface with High Grit & Spin, Elongated Handle, USAPA Approved 2022 Ben Johns Paddle", Price=189.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S008" },
                    new Product{ Title="Pickleball Net", Brand="flybold", Description="flybold Pickleball Net |Portable Set with/without 4 Paddles| Net Regulation Size Equipment Lightweight Sturdy Interlocking Metal Posts with Carrying Bag for Indoor Outdoor Game| Full Court Size - 22ft", Price=99.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S009" },
                    new Product{ Title="Camping Tent", Brand="Coleman", Description="Coleman Sundome Camping Tent, 2/3/4/6 Person Dome Tent with Easy Setup, Included Rainfly and WeatherTec Floor to Block Out Water, 2 Windows and 1 Ground Vent for Air Flow with Charging E-Port Flap", Price=127.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S010" },
                    new Product{ Title="Watersports Tube", Brand="AIRHEAD", Description="AIRHEAD G-Force, 1-4 Rider Towable Tube for Boating, Multiple Size Options Available", Price=142.30M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S011" },
                    new Product{ Title="Stand Up Paddle Board", Brand="Aqua Plus", Description="Aqua Plus 6inches Thick Inflatable SUP for All Skill Levels Stand Up Paddle Board,Paddle,Double Action Pump,ISUP Travel Backpack, Leash,Shoulder Strap,Youth,Adult Inflatable Paddle Board", Price=169.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Sports").Id, ModelNumber="S012" },

                    // Household
                    new Product{ Title="Rivet Aiden Mid-Century Modern Tufted Velvet Loveseat Sofa", Brand="Rivet", Description="This sleek, mid-Century inspired loveseat is designed to impress. ", Price=739.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H001" },
                    new Product{ Title="HERCULES Lesley Series Contemporary Black LeatherSoft Sofa with Encasing Frame", Brand="Flash Furniture", Description="You're great at what you do and your company has grown so make sure you convey that to clients when you choose your reception furniture.", Price=1257.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H002"  },
                    new Product{ Title="Premium Microfiber Cleaning Cloth", Brand="OVWO", Description="80% Polyester/ 20% Polyamide", Price=9.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H003"  },
                    new Product{ Title="Bathroom Cleaner Spray", Brand="Lysol", Description="KILLS 99.9% OF BATHROOM VIRUSES AND BACTERIA", Price=3.97M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H004"  },
                    new Product{ Title="Linen White Bath Towels 4-Pack", Brand="Hammam", Description=" 4-Piece super soft and absorbent Turkish cotton bath towels.", Price=33.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H005"  },
                    new Product{ Title="Sure-Crisp Air Fryer Countertop Toaster Oven", Brand="Hamilton Beach", Description="The Sure-Crisp air fry convection function circulates air around food as it cooks", Price=69.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H005"  },
                    new Product{ Title="Floor Sweeper", Brand="Bissell", Description="Bissell Easy Sweep Compact Carpet & Floor Sweeper", Price=20.59M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H007" },
                    new Product{ Title="Portable Space Heater", Brand="BLACK+DECKER", Description="BLACK+DECKER Portable Space Heater, 1500W Room Space Heater with Carry Handle for Easy Transport", Price=24.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H008" },
                    new Product{ Title="Electric Infrared Quartz Fireplace Stove", Brand="Duraflame", Description="Duraflame Electric Infrared Quartz Fireplace Stove with 3D Flame Effect, Black", Price=219.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H009" },
                    new Product{ Title="Evaporative Air Cooler", Brand="Arctic Air", Description="Arctic Air Pure Chill 2.0 Evaporative Air Cooler by Ontel - Powerful, Quiet, Lightweight and Portable Space Cooler with Hydro-Chill Technology For Bedroom, Office, Living", Price=31.95M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H010" },
                    new Product{ Title="Window Air Conditioner", Brand="LG", Description="LG 6,000 BTU Window Conditioner, Cools 250 Sq.Ft. (10' x 25' Room Size), Quiet Operation, Electronic Control with Remote, 2 Cooling & Fan Speeds, 2-Way Air Deflection, Auto Restart, 115V, White", Price=189.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H011" },
                    new Product{ Title="Humidifier", Brand="Frida Baby", Description="Frida Baby 3-in-1 Humidifier with Diffuser and Nightlight, White", Price=44.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H012" },
                    new Product{ Title="Air Purifier", Brand="LEVOIT", Description="LEVOIT Air Purifiers for Bedroom Home, HEPA Filter Cleaner with Fragrance Sponge for Better Sleep, Filters Smoke, Allergies, Pet Dander, Odor, Dust, Office, Desktop, Portable, Core Mini, White", Price=49.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H013" },
                    new Product{ Title="Area Rug", Brand="Artistic Weavers", Description="Artistic Weavers Odelia Vintage Bohemian Area Rug,Aqua", Price=99.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H014" },
                    new Product{ Title="3-Tier Shoe Rack", Brand="Simple Houseware", Description="Simple Houseware 3-Tier Shoe Rack Storage Organizer 12-Pair / 20-Pair, Bronze", Price=19.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Household").Id, ModelNumber="H015" },


                    // Technology
                    new Product{ Title="Inspiron 3511 Premium Laptop", Brand="Dell", Description="Intel Core i5-1035G1 Quad-Core Processor (4Cores, 6MB Intel Smart Cache, 8 Threads", Price=679.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T001"  },
                    new Product{ Title="XPS 17 9720 Laptop", Brand="Dell", Description="12th Generation Intel Core i7-12700H (24MB Cache, up to 4.7 GHz, 14 cores)", Price=2499.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T002" },
                    new Product{ Title="iPhone 13 Pro", Brand="Apple", Description="6.1-inch Super Retina XDR display with ProMotion for a faster, more responsive feel", Price=999.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T003" },
                    new Product{ Title="iPhone 13 Mini", Brand="Apple", Description="5.4-inch Super Retina XDR display", Price=729.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T004" },
                    new Product{ Title="65-Inch Class UQ9000", Brand="LG", Description="Everything you need to bring your favorite content to life with the power of 4K and the extras you crave.", Price=576.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T005" },
                    new Product{ Title="iPad Air", Brand="Apple", Description="10.9-inch, Wi-Fi + Cellular, 64GB", Price=749.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T006" },
                    new Product{ Title="Wireless Speakers", Brand="Roku", Description="Roku Wireless Speakers (for Roku Streambars or Roku TV)", Price=149.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T007" },
                    new Product{ Title="Headphones", Brand="Sennheiser", Description="Sennheiser HD 400S Closed Back, Around Ear Headphone with One-Button Smart Remote on Detachable Cable", Price=49.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T008" },
                    new Product{ Title="Projector ", Brand="Elephas", Description="Mini Projector for iPhone, ELEPHAS 2023 Upgraded 1080P HD Projector, 8000L Portable Projector with Tripod and Carry Bag, Movie Projector Compatible with Android/iOS/Windows/TV Stick/HDMI/USB", Price=99.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T009" },
                    new Product{ Title="AirTag", Brand="Apple", Description="Apple AirTag", Price=27.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T010" },
                    new Product{ Title="Headphones", Brand="Dyson", Description="Dyson Zone™ headphones", Price=949.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T011" },
                    new Product{ Title="PlayStation 5 Console ", Brand="Sony", Description="PlayStation 5 Console (PS5)", Price=499.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T012" },
                    new Product{ Title="Radar Detector", Brand="Cobra", Description="Cobra RAD 480i Laser Radar Detector – Long Range Detection, Bluetooth, iRadar App, LaserEye Front and Rear Detection, Next Gen IVT Filtering, Black", Price=149.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T013" },
                    new Product{ Title="Dash Cam", Brand="Rove", Description="Rove R2-4K Dash Cam Built in WiFi GPS Car Dashboard Camera Recorder with UHD 2160P", Price=119.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T014" },
                    new Product{ Title="Watch Charger", Brand="Nuinno", Description="Watch Charger for Apple Watch Charger,USB C 20W Fast Charger Adapter 6.6FT", Price=15.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Technology").Id, ModelNumber="T015" },

                    // Fashion
                    new Product{ Title="Mens Tilden Cap Oxford Shoe", Brand="Clarks ", Description="A classic captoe derby crafted from rich, full grain leather.", Price=59.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F001" },
                    new Product{ Title="Mens Cotrell Step Slip-On Loafer", Brand="Clarks", Description="Slip-on casual", Price=54.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F002" },
                    new Product{ Title="Mens Dress Shirt Regular Fit Poplin Solid", Brand="Van Heusen", Description="Fabric features enhanced wrinkle resistance for easy care at home.", Price=24.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F003" },
                    new Product{ Title="Mens Long Sleeve Dress Shirt", Brand="Alberto Danelli", Description="Our cotton blend dress shirt is the perfect fitting day to night closet must have. ", Price=29.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F004" },
                    new Product{ Title="Mens Classic Fit Easy Khaki", Brand="Dockers", Description="64% Cotton, 34% Polyester, 2% Elastane", Price=39.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F005"},
                    new Product{ Title="Mens American Chino Flat Front Straight Fit Pant", Brand="Izod", Description="100% Cotton", Price=31.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F006" },

                    new Product{ Title="Child Rain Boot", Brand="Crocs", Description="Crocs Unisex-Child Rain Boot", Price=19.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F007" },
                    new Product{ Title="Leggings ", Brand="IUGA", Description="IUGA Girls Athletic Leggings with Pockets Running Yoga Pants Girl's Workout Dance Leggings Tights for Girls High Waisted", Price=17.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F008" },
                    new Product{ Title="Skinny Jeans", Brand="Childrens Place", Description="The Childrens Place Girls' Super Skinny Jeans", Price=14.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F009" },
                    new Product{ Title="Jogger Pant", Brand="adidas", Description="adidas Boys' Iconic Tricot Jogger Pant", Price=28.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F010" },
                    new Product{ Title="Sunglasses", Brand="Oakley", Description="Oakley Men's OO9096 Fuel Cell Wrap Sunglasses", Price=59.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F011" },
                    new Product{ Title="Flip-Flop", Brand="Reef", Description="Reef Men's Phantoms Flip-Flop", Price=22.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F012" },
                    new Product{ Title="Men’s Swim Trunks", Brand="Body Glove", Description="Body Glove Men’s Swim Trunks – Stretch Fit Bathing Suit for Men", Price=16.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F013" },
                    new Product{ Title="Polo Shirt", Brand="Nautica", Description="Nautica Mens Classic Short Sleeve Solid Performance Deck Polo Shirt", Price=26.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F014" },
                    new Product{ Title="Cotton Boxers", Brand="POLO", Description="POLO Ralph Lauren Men's Classic Fit Cotton Woven Boxers 3-Pack", Price=41.99M, AvailableQty=30, ProductCategoryId=context.ProductCategory.Single(x => x.Title == "Fashion").Id, ModelNumber="F015" },
                };


                foreach (var item in products)
                    context.Product.Add(item);

                context.SaveChanges();
            }
            #endregion

            #region order status
            if (!context.OrderStatus.Any())
            {
                var orderStatus = new OrderStatus[]
                {
                    new OrderStatus { Name = "Cart"},
                    new OrderStatus { Name = "Processing"},
                    new OrderStatus { Name = "Shipped"},
                    new OrderStatus { Name = "Delivered"},
                    new OrderStatus { Name = "Returned"},
                };

                foreach (var item in orderStatus)
                    context.OrderStatus.Add(item);

                context.SaveChanges();
            }
            #endregion

            #region states
            // US States
            if (!context.StateLocation.Any())
            {
                var states = new StateLocation[]
                {
                    new StateLocation{Name = "Alabama", Abbreviation = "AL", TaxRate = 0.04M },
                    new StateLocation{Name = "Alaska", Abbreviation = "AK", TaxRate = 0.05M },
                    new StateLocation{Name = "Arizona", Abbreviation = "AZ", TaxRate = 0.06M },
                    new StateLocation{Name = "Arkansas", Abbreviation = "AR", TaxRate = 0.07M },
                    new StateLocation{Name = "California", Abbreviation = "CA", TaxRate = 0.08M },
                    new StateLocation{Name = "Colorado", Abbreviation = "CO", TaxRate = 0.07M },
                    new StateLocation{Name = "Connecticut", Abbreviation = "CT", TaxRate = 0.06M },
                    new StateLocation{Name = "Delaware", Abbreviation = "DE", TaxRate = 0.05M },
                    new StateLocation{Name = "Florida", Abbreviation = "FL", TaxRate = 0.04M },
                    new StateLocation{Name = "Georgia", Abbreviation = "GA", TaxRate = 0.05M },
                    new StateLocation{Name = "Hawaii", Abbreviation = "HI", TaxRate = 0.06M },
                    new StateLocation{Name = "Idaho", Abbreviation = "ID", TaxRate = 0.07M },
                    new StateLocation{Name = "Illinois", Abbreviation = "IL", TaxRate = 0.08M },
                    new StateLocation{Name = "Indiana", Abbreviation = "IN", TaxRate = 0.07M },
                    new StateLocation{Name = "Iowa", Abbreviation = "IA", TaxRate = 0.06M },
                    new StateLocation{Name = "Kansas", Abbreviation = "KS", TaxRate = 0.05M },
                    new StateLocation{Name = "Kentucky", Abbreviation = "KY", TaxRate = 0.04M },
                    new StateLocation{Name = "Louisiana", Abbreviation = "LA", TaxRate = 0.05M },
                    new StateLocation{Name = "Maine", Abbreviation = "ME", TaxRate = 0.06M },
                    new StateLocation{Name = "Maryland", Abbreviation = "MD", TaxRate = 0.07M },
                    new StateLocation{Name = "Massachusetts", Abbreviation = "MA", TaxRate = 0.08M },
                    new StateLocation{Name = "Michigan", Abbreviation = "MI", TaxRate = 0.07M },
                    new StateLocation{Name = "Minnesota", Abbreviation = "MN", TaxRate = 0.06M },
                    new StateLocation{Name = "Mississippi", Abbreviation = "MS", TaxRate = 0.05M },
                    new StateLocation{Name = "Missouri", Abbreviation = "MO", TaxRate = 0.04M },
                    new StateLocation{Name = "Montana", Abbreviation = "MT", TaxRate = 0.05M },
                    new StateLocation{Name = "Nebraska", Abbreviation = "NE", TaxRate = 0.06M },
                    new StateLocation{Name = "Nevada", Abbreviation = "NV", TaxRate = 0.07M },
                    new StateLocation{Name = "New Hampshire", Abbreviation = "NH", TaxRate = 0.08M },
                    new StateLocation{Name = "New Jersey", Abbreviation = "NJ", TaxRate = 0.08M },
                    new StateLocation{Name = "New Mexico", Abbreviation = "NM", TaxRate = 0.07M },
                    new StateLocation{Name = "New York", Abbreviation = "NY", TaxRate = 0.06M },
                    new StateLocation{Name = "North Carolina", Abbreviation = "NC", TaxRate = 0.05M },
                    new StateLocation{Name = "North Dakota", Abbreviation = "ND", TaxRate = 0.04M },
                    new StateLocation{Name = "Ohio", Abbreviation = "OH", TaxRate = 0.08M },
                    new StateLocation{Name = "Oklahoma", Abbreviation = "OK", TaxRate = 0.04M },
                    new StateLocation{Name = "Oregon", Abbreviation = "OR", TaxRate = 0.05M },
                    new StateLocation{Name = "Pennsylvania", Abbreviation = "PA", TaxRate = 0.08M },
                    new StateLocation{Name = "Rhode Island", Abbreviation = "RI", TaxRate = 0.07M },
                    new StateLocation{Name = "South Carolina", Abbreviation = "SC", TaxRate = 0.04M },
                    new StateLocation{Name = "South Dakota", Abbreviation = "SD", TaxRate = 0.04M },
                    new StateLocation{Name = "Tennessee", Abbreviation = "TN", TaxRate = 0.04M },
                    new StateLocation{Name = "Texas", Abbreviation = "TX", TaxRate = 0.0M },
                    new StateLocation{Name = "Utah", Abbreviation = "UT", TaxRate = 0.05M },
                    new StateLocation{Name = "Vermont", Abbreviation = "VT", TaxRate = 0.05M },
                    new StateLocation{Name = "Virginia", Abbreviation = "VA", TaxRate = 0.06M },
                    new StateLocation{Name = "Washington", Abbreviation = "WA", TaxRate = 0.04M },
                    new StateLocation{Name = "West Virginia", Abbreviation = "WV", TaxRate = 0.03M },
                    new StateLocation{Name = "Wisconsin", Abbreviation = "WI", TaxRate = 0.06M },
                    new StateLocation{Name = "Wyoming", Abbreviation = "WY", TaxRate = 0.00M },
                    new StateLocation{Name = "District of Columbia", Abbreviation = "DC", TaxRate = 0.06M },
                    new StateLocation{Name = "Guam", Abbreviation = "GU", TaxRate = 0.06M },
                    new StateLocation{Name = "Marshall Islands", Abbreviation = "MH", TaxRate = 0.06M },
                    new StateLocation{Name = "Northern Mariana Island", Abbreviation = "MP", TaxRate = 0.06M },
                    new StateLocation{Name = "Puerto Rico", Abbreviation = "PR", TaxRate = 0.06M },
                    new StateLocation{Name = "Virgin Islands", Abbreviation = "VI", TaxRate = 0.06M }
                };

                foreach (var item in states)
                    context.StateLocation.Add(item);

                context.SaveChanges();
            }
            #endregion

            
            #region users

            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                if (roleStore != null)
                {
                    string[] roles = new string[] { "ADMIN", "CUSTOMER" };
                    foreach (string role in roles)
                    {
                        await roleStore.CreateAsync(new IdentityRole { Name = role, NormalizedName = role });
                    }
                }

                await context.SaveChangesAsync();
            }


            if (!context.Users.Any())
            {
                UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(context);
                var hasher = new PasswordHasher<IdentityUser>();

                if (userStore != null && hasher != null)
                {
                    // admin
                    var adminUser = new ApplicationUser
                    {
                        UserName = "administrator",
                        NormalizedUserName = "ADMINISTRATOR",
                        FirstName = "Ad",
                        LastName = "Min",
                        Address1 = "100 Main",
                        Address2 = "#1",
                        City = "Longmont",
                        StateLocationId = context.StateLocation.Single(x => x.Abbreviation == "CO").Id,
                        PostalCode = "80501",
                        PhoneNumber = "111-111-1111",
                        Email = "admin@commercedemo.com",
                        NormalizedEmail = "ADMIN@COMMERCEDEMO.COM",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };

                    adminUser.PasswordHash = hasher.HashPassword(adminUser, "password");
                    await userStore.CreateAsync(adminUser);
                    await userStore.AddToRoleAsync(adminUser, "ADMIN");


                    // customers
                    var customers = new ApplicationUser[]
                    {
                        new ApplicationUser{UserName = "jerry", NormalizedUserName = "JERRY", FirstName = "Jerry", LastName="Seinfeld", Address1="151 5th Avenue", Address2="#201", City="New York", StateLocationId=context.StateLocation.Single(x=>x.Abbreviation=="NY").Id, PostalCode="10001", PhoneNumber="212-555-1234", Email="jerry@seinfeld.com", NormalizedEmail = "JERRY@SEINFELD.COM", EmailConfirmed = true, PhoneNumberConfirmed = true, SecurityStamp = Guid.NewGuid().ToString("D")},
                        new ApplicationUser{UserName = "elaine", NormalizedUserName = "ELAINE", FirstName = "Elaine", LastName="Benes", Address1="325 Columbus Avenue", Address2="#12", City="New York", StateLocationId=context.StateLocation.Single(x=>x.Abbreviation=="NY").Id, PostalCode="10001", PhoneNumber="212-555-2345", Email="ELAINE@SEINFELD.COM", NormalizedEmail = "elaine@seinfeld.com", EmailConfirmed = true, PhoneNumberConfirmed = true, SecurityStamp = Guid.NewGuid().ToString("D")},
                        new ApplicationUser{UserName = "kramer", NormalizedUserName = "KRAMER", FirstName = "Cosmo", LastName="Kramer", Address1="151 5th Avenue", Address2="#202", City="New York", StateLocationId=context.StateLocation.Single(x=>x.Abbreviation=="NY").Id, PostalCode="10001", PhoneNumber="212-555-3456", Email="KRAMER@SEINFELD.COM", NormalizedEmail = "kramer@seinfeld.com", EmailConfirmed = true, PhoneNumberConfirmed = true, SecurityStamp = Guid.NewGuid().ToString("D")},
                        new ApplicationUser{UserName = "george", NormalizedUserName = "GEORGE", FirstName = "George", LastName="Costanza", Address1="525 42nd St", Address2="#604", City="New York", StateLocationId=context.StateLocation.Single(x=>x.Abbreviation=="NY").Id, PostalCode="10001", PhoneNumber="212-555-4567", Email="GEORGE@SEINFELD.COM", NormalizedEmail = "george@seinfeld.com", EmailConfirmed = true, PhoneNumberConfirmed = true, SecurityStamp = Guid.NewGuid().ToString("D")},
                        new ApplicationUser{UserName = "newman", NormalizedUserName = "NEWMAN", FirstName = "N", LastName="Newman", Address1="151 5th Avenue", Address2="#5", City="New York", StateLocationId=context.StateLocation.Single(x=>x.Abbreviation=="NY").Id, PostalCode="10001", PhoneNumber="212-555-5678", Email="newman@seinfeld.com", NormalizedEmail = "NEWMAN@SEINFELD.COM",EmailConfirmed = true, PhoneNumberConfirmed = true, SecurityStamp = Guid.NewGuid().ToString("D")}
                    };

                    foreach (var user in customers)
                    {
                        user.PasswordHash = hasher.HashPassword(user, "password");
                        await userStore.CreateAsync(user);
                        await userStore.AddToRoleAsync(user, "CUSTOMER");
                    }

                    await context.SaveChangesAsync();

                }

            }


            #endregion

            

        }


    }
}
