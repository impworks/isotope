﻿// <auto-generated />
using System;
using Isotope.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Isotope.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("Isotope.Data.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Isotope.Data.Models.DynamicConfigWrapper", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DynamicConfig");
                });

            modelBuilder.Entity("Isotope.Data.Models.Folder", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Caption")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<int>("Depth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediaCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<string>("ThumbnailKey")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.HasIndex("Path")
                        .IsUnique();

                    b.HasIndex("ThumbnailKey");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("Isotope.Data.Models.FolderTagBinding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FolderKey");

                    b.HasIndex("TagId");

                    b.ToTable("FolderTags");
                });

            modelBuilder.Entity("Isotope.Data.Models.Media", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Date")
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Key");

                    b.HasIndex("FolderKey");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("Isotope.Data.Models.MediaTagBinding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MediaKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MediaKey");

                    b.HasIndex("TagId");

                    b.ToTable("MediaTags");
                });

            modelBuilder.Entity("Isotope.Data.Models.SharedLink", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("DateFrom")
                        .HasColumnType("TEXT");

                    b.Property<string>("DateTo")
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderKey")
                        .HasColumnType("TEXT");

                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tags")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.HasIndex("FolderKey");

                    b.ToTable("SharedLinks");
                });

            modelBuilder.Entity("Isotope.Data.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Caption")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Isotope.Data.Models.TagHash", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("TagHashes");
                });

            modelBuilder.Entity("Isotope.Data.Models.TagSuggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<string>("MediaKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MediaKey");

                    b.HasIndex("TagId");

                    b.ToTable("TagSuggestions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Isotope.Data.Models.Folder", b =>
                {
                    b.HasOne("Isotope.Data.Models.Media", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailKey");
                });

            modelBuilder.Entity("Isotope.Data.Models.FolderTagBinding", b =>
                {
                    b.HasOne("Isotope.Data.Models.Folder", "Folder")
                        .WithMany("Tags")
                        .HasForeignKey("FolderKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Isotope.Data.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");
                });

            modelBuilder.Entity("Isotope.Data.Models.Media", b =>
                {
                    b.HasOne("Isotope.Data.Models.Folder", "Folder")
                        .WithMany()
                        .HasForeignKey("FolderKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Isotope.Data.Models.Rect", "ThumbnailRect", b1 =>
                        {
                            b1.Property<string>("MediaKey")
                                .HasColumnType("TEXT");

                            b1.Property<double>("Height")
                                .HasColumnType("REAL");

                            b1.Property<double>("Width")
                                .HasColumnType("REAL");

                            b1.Property<double>("X")
                                .HasColumnType("REAL");

                            b1.Property<double>("Y")
                                .HasColumnType("REAL");

                            b1.HasKey("MediaKey");

                            b1.ToTable("Media");

                            b1.WithOwner()
                                .HasForeignKey("MediaKey");
                        });
                });

            modelBuilder.Entity("Isotope.Data.Models.MediaTagBinding", b =>
                {
                    b.HasOne("Isotope.Data.Models.Media", "Media")
                        .WithMany("Tags")
                        .HasForeignKey("MediaKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Isotope.Data.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");

                    b.OwnsOne("Isotope.Data.Models.Rect", "Location", b1 =>
                        {
                            b1.Property<int>("MediaTagBindingId")
                                .HasColumnType("INTEGER");

                            b1.Property<double>("Height")
                                .HasColumnType("REAL");

                            b1.Property<double>("Width")
                                .HasColumnType("REAL");

                            b1.Property<double>("X")
                                .HasColumnType("REAL");

                            b1.Property<double>("Y")
                                .HasColumnType("REAL");

                            b1.HasKey("MediaTagBindingId");

                            b1.ToTable("MediaTags");

                            b1.WithOwner()
                                .HasForeignKey("MediaTagBindingId");
                        });
                });

            modelBuilder.Entity("Isotope.Data.Models.SharedLink", b =>
                {
                    b.HasOne("Isotope.Data.Models.Folder", "Folder")
                        .WithMany()
                        .HasForeignKey("FolderKey");
                });

            modelBuilder.Entity("Isotope.Data.Models.TagHash", b =>
                {
                    b.HasOne("Isotope.Data.Models.Tag", "Tag")
                        .WithMany("Hashes")
                        .HasForeignKey("TagId");
                });

            modelBuilder.Entity("Isotope.Data.Models.TagSuggestion", b =>
                {
                    b.HasOne("Isotope.Data.Models.Media", "Media")
                        .WithMany()
                        .HasForeignKey("MediaKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Isotope.Data.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");

                    b.OwnsOne("Isotope.Data.Models.Rect", "Location", b1 =>
                        {
                            b1.Property<int>("TagSuggestionId")
                                .HasColumnType("INTEGER");

                            b1.Property<double>("Height")
                                .HasColumnType("REAL");

                            b1.Property<double>("Width")
                                .HasColumnType("REAL");

                            b1.Property<double>("X")
                                .HasColumnType("REAL");

                            b1.Property<double>("Y")
                                .HasColumnType("REAL");

                            b1.HasKey("TagSuggestionId");

                            b1.ToTable("TagSuggestions");

                            b1.WithOwner()
                                .HasForeignKey("TagSuggestionId");
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Isotope.Data.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Isotope.Data.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Isotope.Data.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Isotope.Data.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
