using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Primitives;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;

namespace Charts.Shared.Data.Context
{
    public class DataContextInitializer
    {
        public static void SeedRolesAndPermissions(DataContext context)
        {
            try
            {
                foreach (var permission in Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>())
                {
                    if (!context.Roles.Any(x => x.Value == permission))
                        context.Roles.Add(new Role
                        {
                            Value = permission
                        });
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void FillRapair(string path, DataContext context)
        {
            if (path != null)
            {
                try
                {
                    FileStream stream = File.Open(path + "Справочник_Место_ремонта.xls", FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);


                    while (excelReader.Read())
                    {
                        try
                        {
                            DicRepairPlace valuePropertyOfObject = new DicRepairPlace();

                            var name = excelReader.GetValue(0);
                            if (name != null)
                            {
                                valuePropertyOfObject.NameRu = name.ToString().Trim();
                            }
                            else
                            {
                                valuePropertyOfObject.NameRu = string.Empty;
                            }
                            context.DicRepairPlace.AddAsync(valuePropertyOfObject);
                        }
                        catch (Exception exception)
                        {
                            string ex = exception.Message;
                            throw;
                        }
                    }
                    context.SaveChanges();
                    excelReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        private static void FillContragents(string path, DataContext context)
        {
            if (path != null)
            {
                try
                {
                    FileStream stream = File.Open(path + "Справочник_контрагенты.xls", FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);


                    while (excelReader.Read())
                    {
                        try
                        {
                            DicContractors valuePropertyOfObject = new DicContractors();

                            var name = excelReader.GetValue(0);
                            if (name != null)
                            {
                                valuePropertyOfObject.NameRu = name.ToString().Trim();
                            }
                            else
                            {
                                valuePropertyOfObject.NameRu = string.Empty;
                            }
                            context.DicContractors.AddAsync(valuePropertyOfObject);
                        }
                        catch (Exception exception)
                        {
                            string ex = exception.Message;
                            throw;
                        }
                    }
                    context.SaveChanges();
                    excelReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        private static void FillDefects(string path, DataContext context)
        {
            if (path != null)
            {
                try
                {
                    FileStream stream = File.Open(path + "Справочник_неисправностей.xls", FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);


                    while (excelReader.Read())
                    {
                        try
                        {
                            DicDefect valuePropertyOfObject = new DicDefect();

                            var name = excelReader.GetValue(0);
                            if (name != null)
                            {
                                valuePropertyOfObject.NameRu = name.ToString().Trim();
                            }
                            else
                            {
                                valuePropertyOfObject.NameRu = string.Empty;
                            }
                            context.DicDefect.AddAsync(valuePropertyOfObject);
                        }
                        catch (Exception exception)
                        {
                            string ex = exception.Message;
                            throw;
                        }
                    }
                    context.SaveChanges();
                    excelReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static void SeedDictionaries(DataContext context, IConfiguration configuration)
        {
            var path = configuration.GetSection("AppSettings:Dictionaries")["Path"];
            FillRapair(path, context);
            FillContragents(path, context);
            FillDefects(path, context);
        }

        public static void SeedAdminUser(DataContext context, IConfiguration configuration)
        {
            try
            {
                if (!context.Roles.Any(x => x.Value == RoleEnum.Admin))
                    throw new Exception("Сначало необходимо вызвать метод: " + nameof(SeedRolesAndPermissions));

                if (!context.Users.Where(x => x.UserRoles.Any(ur => ur.Role.Value == RoleEnum.Admin)).Any())
                {
                    var role = context.Roles.First(x => x.Value == RoleEnum.Admin);
                    var password = configuration.GetSection("AppSettings:Admin")["Password"];
                    if (string.IsNullOrEmpty(password))
                        throw new Exception("Не указан путь AppSettings:Admin:Password в appsettings.{config}.json");

                    var user = new User
                    {
                        Email = "admin@charts.kz",
                        LastName = "Администратор",
                        Login = "admin@charts.kz",
                        Password = HashPwd(password),
                        Audience = PortalEnum.Int
                    };

                    user.UserRoles.Add(new UserRole
                    {
                        RoleId = role.Id
                    });

                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        private static string HashPwd(string pwd)
        {
            var alg = SHA512.Create();
            alg.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            return Convert.ToBase64String(alg.Hash);
        }
    }
}