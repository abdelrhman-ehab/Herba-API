using HerbCare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace HerbCare.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
       // public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<UserExercise> UserExercises { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<UserQuiz> UserQuizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionQuiz> QuestionQuizzes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Herb> Herbs { get; set; }
        public DbSet<UserHerb> UserHerbs { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreLocation> StoreLocations { get; set; }
        public DbSet<Models.UserStore> UserStores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }
        public DbSet<HerbStore> HerbStores { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<IdealWeightCalculation> IdealWeightCalculations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== APPLICATION USER CONFIGURATION ==========
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_ApplicationUser_Email");
                entity.HasIndex(u => u.UserType).HasDatabaseName("IX_ApplicationUser_UserType");

                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Phone).HasMaxLength(20);
                entity.Property(u => u.Specialty).HasMaxLength(100);
                entity.Property(u => u.Rating).HasDefaultValue(0);
            });

            // ========== DOCTOR CONFIGURATION ==========
            //modelBuilder.Entity<Doctor>(entity =>
            //{
            //    entity.HasKey(d => d.DoctorId);
            //    entity.HasIndex(d => d.Email).IsUnique().HasDatabaseName("IX_Doctor_Email");

            //    entity.Property(d => d.FirstName).IsRequired().HasMaxLength(50);
            //    entity.Property(d => d.LastName).IsRequired().HasMaxLength(50);
            //    entity.Property(d => d.Specialty).IsRequired().HasMaxLength(100);
            //    entity.Property(d => d.Phone).HasMaxLength(20);
            //    entity.Property(d => d.Rating).HasDefaultValue(0);

            //    // Relationship with ApplicationUser (if needed)
            //    entity.HasOne(d => d.User)
            //        .WithMany()
            //        .HasForeignKey("UserId")
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            // ========== USER EXERCISE (Many-to-Many) ==========
            modelBuilder.Entity<UserExercise>(entity =>
            {
                entity.HasKey(ue => new { ue.UserId, ue.ExerciseId });

                entity.HasOne(ue => ue.User)
                    .WithMany(u => u.UserExercises)
                    .HasForeignKey(ue => ue.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ue => ue.Exercise)
                    .WithMany(e => e.UserExercises)
                    .HasForeignKey(ue => ue.ExerciseId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ue => ue.UserId).HasDatabaseName("IX_UserExercise_UserId");
                entity.HasIndex(ue => ue.ExerciseId).HasDatabaseName("IX_UserExercise_ExerciseId");
            });

            // ========== USER QUIZ (Many-to-Many) ==========
            modelBuilder.Entity<UserQuiz>(entity =>
            {
                entity.HasKey(uq => new { uq.UserId, uq.QuizId });

                entity.HasOne(uq => uq.User)
                    .WithMany(u => u.UserQuizzes)
                    .HasForeignKey(uq => uq.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uq => uq.Quiz)
                    .WithMany(q => q.UserQuizzes)
                    .HasForeignKey(uq => uq.QuizId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(uq => uq.UserId).HasDatabaseName("IX_UserQuiz_UserId");
                entity.HasIndex(uq => uq.QuizId).HasDatabaseName("IX_UserQuiz_QuizId");
            });

            // ========== QUESTION QUIZ (Many-to-Many) ==========
            modelBuilder.Entity<QuestionQuiz>(entity =>
            {
                entity.HasKey(qq => new { qq.QuestionId, qq.QuizId });

                entity.HasOne(qq => qq.Question)
                    .WithMany(q => q.QuestionQuizzes)
                    .HasForeignKey(qq => qq.QuestionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(qq => qq.Quiz)
                    .WithMany(q => q.QuestionQuizzes)
                    .HasForeignKey(qq => qq.QuizId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== USER HERB (Many-to-Many) ==========
            modelBuilder.Entity<UserHerb>(entity =>
            {
                entity.HasKey(uh => new { uh.UserId, uh.HerbId });

                entity.HasOne(uh => uh.User)
                    .WithMany(u => u.UserHerbs)
                    .HasForeignKey(uh => uh.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uh => uh.Herb)
                    .WithMany(h => h.UserHerbs)
                    .HasForeignKey(uh => uh.HerbId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(uh => uh.UserId).HasDatabaseName("IX_UserHerb_UserId");
                entity.HasIndex(uh => uh.HerbId).HasDatabaseName("IX_UserHerb_HerbId");
            });

            // ========== USER STORE (Many-to-Many) ==========
            modelBuilder.Entity<Models.UserStore>(entity =>
            {
                entity.HasKey(us => new { us.UserId, us.StoreId });

                entity.HasOne(us => us.User)
                    .WithMany(u => u.UserStores)
                    .HasForeignKey(us => us.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(us => us.Store)
                    .WithMany(s => s.UserStores)
                    .HasForeignKey(us => us.StoreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== STORE PRODUCT (Many-to-Many) ==========
            modelBuilder.Entity<StoreProduct>(entity =>
            {
                entity.HasKey(sp => new { sp.StoreId, sp.ProductId });

                entity.HasOne(sp => sp.Store)
                    .WithMany(s => s.StoreProducts)
                    .HasForeignKey(sp => sp.StoreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.Product)
                    .WithMany(p => p.StoreProducts)
                    .HasForeignKey(sp => sp.ProductId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== HERB STORE (Many-to-Many) ==========
            modelBuilder.Entity<HerbStore>(entity =>
            {
                entity.HasKey(hs => new { hs.HerbId, hs.StoreId });

                entity.HasOne(hs => hs.Herb)
                    .WithMany(h => h.HerbStores)
                    .HasForeignKey(hs => hs.HerbId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(hs => hs.Store)
                    .WithMany(s => s.HerbStores)
                    .HasForeignKey(hs => hs.StoreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== STORE LOCATION ==========
            modelBuilder.Entity<StoreLocation>(entity =>
            {
                entity.HasKey(sl => sl.StoreLocationId);

                entity.HasOne(sl => sl.Store)
                    .WithMany(s => s.StoreLocations)
                    .HasForeignKey(sl => sl.StoreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(sl => sl.Location).IsRequired().HasMaxLength(200);
                entity.HasIndex(sl => sl.StoreId).HasDatabaseName("IX_StoreLocation_StoreId");
            });

            // ========== CART ==========
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.CartId);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Carts)
                    .HasForeignKey(c => c.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Quantity).HasDefaultValue(0);
                entity.Property(c => c.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Add Status property if not in model
                entity.Property<string>("Status").HasDefaultValue("Active");

                // Ensure one active cart per user
                entity.HasIndex(c => c.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_Cart_User_Unique")
                    .HasFilter("[Status] = 'Active'");
            });

            // ========== CART ITEM ==========
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.CartItemId);

                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ci => ci.Quantity).IsRequired().HasDefaultValue(1);
                entity.HasIndex(ci => ci.CartId).HasDatabaseName("IX_CartItem_CartId");
                entity.HasIndex(ci => ci.ProductId).HasDatabaseName("IX_CartItem_ProductId");
            });

            // ========== ORDER PAYMENT ==========
            modelBuilder.Entity<OrderPayment>(entity =>
            {
                entity.HasKey(op => op.PaymentId);

                entity.HasOne(op => op.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(op => op.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(op => op.Cart)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(op => op.CartId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(op => op.OrderId).IsRequired();
                entity.Property(op => op.TotalAmount).HasPrecision(18, 2);
                entity.Property(op => op.Date).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(op => op.PaymentMethod).HasMaxLength(50);

                entity.HasIndex(op => op.OrderId).IsUnique().HasDatabaseName("IX_OrderPayment_OrderId");
                entity.HasIndex(op => new { op.UserId, op.Date }).HasDatabaseName("IX_OrderPayment_User_Date");
                entity.HasIndex(op => op.Date).HasDatabaseName("IX_OrderPayment_Date");
            });

            // ========== CONSULTATION CONFIGURATION ==========
            modelBuilder.Entity<Consultation>(entity =>
            {
                entity.HasKey(c => c.ConId);

                // User relationship
                entity.HasOne(c => c.User)
                    .WithMany(u => u.UserConsultations)
                    .HasForeignKey(c => c.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Doctor relationship - now referencing ApplicationUser
                entity.HasOne(c => c.Doctor)
                    .WithMany(u => u.DoctorConsultations)
                    .HasForeignKey(c => c.DoctorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Date).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.Message).HasMaxLength(1000);
                entity.Property(c => c.Reply).HasMaxLength(1000);

                entity.HasIndex(c => c.UserId).HasDatabaseName("IX_Consultation_User");
                entity.HasIndex(c => c.DoctorId).HasDatabaseName("IX_Consultation_Doctor");
                entity.HasIndex(c => c.Date).HasDatabaseName("IX_Consultation_Date");
            });

            // ========== FAVORITE ==========
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(f => f.FavId);

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Herb)
                    .WithMany(h => h.Favorites)
                    .HasForeignKey(f => f.HerbId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.Date).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(f => f.Add).HasDefaultValue(true);

                // Ensure unique favorite per user/herb
                entity.HasIndex(f => new { f.UserId, f.HerbId })
                    .IsUnique()
                    .HasDatabaseName("IX_Favorite_User_Herb");

                entity.HasIndex(f => f.UserId).HasDatabaseName("IX_Favorite_UserId");
                entity.HasIndex(f => f.HerbId).HasDatabaseName("IX_Favorite_HerbId");
            });

            // ========== CATEGORY ==========
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.HasIndex(c => c.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Category_Name");

                entity.HasMany(c => c.Herbs)
                    .WithOne(h => h.Category)
                    .HasForeignKey(h => h.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ========== HERB ==========
            modelBuilder.Entity<Herb>(entity =>
            {
                entity.HasKey(h => h.HerbId);

                entity.Property(h => h.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(h => h.Price)
                    .HasPrecision(18, 2);

                entity.Property(h => h.Benefits)
                    .HasMaxLength(1000);

                entity.Property(h => h.Description)
                    .HasMaxLength(2000);

                entity.Property(h => h.SideEffects)
                    .HasMaxLength(1000);

                entity.HasIndex(h => h.Name)
                    .HasDatabaseName("IX_Herb_Name");

                entity.HasIndex(h => h.CategoryId)
                    .HasDatabaseName("IX_Herb_Category");
            });

            // ========== PRODUCT ==========
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);

                entity.Property(p => p.Image).HasMaxLength(2000);

                entity.HasIndex(p => p.Name)
                    .HasDatabaseName("IX_Product_Name");
            });

            // ========== STORE ==========
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.StoreId);

                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(s => s.ContactInfo)
                    .HasMaxLength(500);

                entity.HasIndex(s => s.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Store_Name");
            });

            // ========== QUIZ ==========
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(q => q.QuizId);

                entity.Property(q => q.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(q => q.TotalPoints)
                    .HasDefaultValue(0);
            });

            // ========== QUESTION ==========
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.QuestionId);
                entity.Property(q => q.Text).IsRequired().HasMaxLength(500);

                entity.Property(q => q.Correct)
                    .HasMaxLength(1); // "A" or "B" or "C" or "D"
            });
            // ========== EXERCISE ==========
            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.ExerciseId);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);


                entity.Property(e => e.Duration)
                    .HasDefaultValue(30);
            });
            // ========== IDEAL WEIGHT CALCULATION ==========
            modelBuilder.Entity<IdealWeightCalculation>(entity =>
            {
                entity.HasKey(iw => iw.Id);

                entity.HasOne(iw => iw.User)
                    .WithMany(u => u.IdealWeightCalculations)
                    .HasForeignKey(iw => iw.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(iw => iw.Gender).IsRequired().HasMaxLength(10);
                entity.Property(iw => iw.Height).IsRequired();
                entity.Property(iw => iw.Age).IsRequired();
                entity.Property(iw => iw.IdealWeight).IsRequired();
                entity.Property(iw => iw.CalculationMethod).HasMaxLength(50);

                entity.HasIndex(iw => iw.UserId).HasDatabaseName("IX_IdealWeightCalculation_UserId");
                entity.HasIndex(iw => iw.CalculationDate).HasDatabaseName("IX_IdealWeightCalculation_Date");
            });
        }
    }
}