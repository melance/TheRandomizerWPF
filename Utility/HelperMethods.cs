using System;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Utility
{
    public sealed class HelperMethods
    {
        public static Font ConvertToFont(string text)
        {
            var fc = new FontConverter();
            if (fc.IsValid(text))
            {
                return ((Font) (fc.ConvertFromString(text)));
            }
            return null;
        }

        public static string ConvertFromFont(Font font)
        {
            var fc = new FontConverter();
            return fc.ConvertToString(font);
        }

        public static string GetPropertyName<TModel, TValue>(Expression<Func<TModel, TValue>> propertyId)
        {
            var member = ((MemberExpression) propertyId.Body).Member;
            return member.Name;
        }

        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
            {
                return false;
            }
            var accessRules = accessControlList.GetAccessRules(true, true, typeof (SecurityIdentifier));
            if (accessRules == null)
            {
                return false;
            }

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }
    }
}