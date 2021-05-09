﻿// <auto-generated />
using System;
using CaffeMenuBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CaffeMenuBot.Data.Migrations
{
    [DbContext(typeof(CaffeMenuBotContext))]
    [Migration("20210509000353_BackToJwt")]
    partial class BackToJwt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Authentication.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_role");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_salt");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("app_users", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Bot.BotUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("user_phone");

                    b.HasKey("Id");

                    b.ToTable("bot_users", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Menu.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("category_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category_name");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("categories", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Menu.Dish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("dish_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("DishName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("dish_name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(5,2)")
                        .HasColumnName("price");

                    b.Property<string>("Serving")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("serving");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("dishes", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Reviews.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("review_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte>("Rating")
                        .HasColumnType("smallint")
                        .HasColumnName("rating");

                    b.Property<string>("ReviewComment")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("review_comment");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("reviews", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Schedule.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("schedule_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CloseTime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("close_time");

                    b.Property<string>("OpenTime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("open_time");

                    b.Property<string>("ScheduleName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("weekday_name");

                    b.HasKey("Id");

                    b.ToTable("schedule", "public");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Menu.Category", b =>
                {
                    b.HasOne("CaffeMenuBot.Data.Models.Menu.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Menu.Dish", b =>
                {
                    b.HasOne("CaffeMenuBot.Data.Models.Menu.Category", "Category")
                        .WithMany("Dishes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Reviews.Review", b =>
                {
                    b.HasOne("CaffeMenuBot.Data.Models.Bot.BotUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CaffeMenuBot.Data.Models.Menu.Category", b =>
                {
                    b.Navigation("Dishes");

                    b.Navigation("SubCategories");
                });
#pragma warning restore 612, 618
        }
    }
}