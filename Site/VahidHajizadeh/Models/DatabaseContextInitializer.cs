using System;

namespace Models
{
    internal static class DatabaseContextInitializer
    {
        static DatabaseContextInitializer()
        {

        }

        internal static void Seed(DatabaseContext databaseContext)
        {
           InitialRoles(databaseContext);
        }

        #region Role
        public static void InitialRoles(DatabaseContext databaseContext)
        {
            InsertRole("ab25c08e-611a-42c0-9025-52269d5acbfd", "SuperAdministrator", "راهبر ویژه", databaseContext);
            InsertRole("3c30af2c-b654-4f07-b7ee-2ca38a5501d7", "Administrator", "راهبر", databaseContext);
            InsertRole("2d994cc5-6b11-453f-883e-94874978dce3", "customer", "مشتری", databaseContext);
        }

        public static void InsertRole(string roleId, string roleName, string roleTitle, DatabaseContext databaseContext)
        {
            Guid id = new Guid(roleId);
            Role role = new Role();
            role.Id = id;
            role.Title = roleTitle;
            role.Name = roleName;
            role.CreationDate = DateTime.Now;
            role.IsActive = true;
            role.IsDeleted = false;

            databaseContext.Roles.Add(role);
            databaseContext.SaveChanges();
        }
        #endregion

        #region ProductType
   

      
        #endregion


    }
}
