// <copyright file="ObjectExtensions.cs" company="Matt Brailsford">
// Copyright (c) 2017 Matt Brailsford and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Fluidity.Extensions
{
    internal static class ObjectExtensions
    {
        public static object GetPropertyValue(this object instance, PropertyInfo propertyInfo)
        {
            return GetPropertyValue(instance, propertyInfo.Name);
        }

        public static object GetPropertyValue(this object instance, string propertyName)
        {
            var obj = instance;

            string[] bits = propertyName.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = obj.GetType().GetProperty(bits[i]);

                var currentVal = propertyToGet.GetValue(obj, null);

                if (currentVal == null)
                {
                    return null;
                }

                obj = propertyToGet.GetValue(obj, null);
            }

            return obj.GetType().GetPropertyValue(bits.Last(), obj);
        }

        public static void SetPropertyValue(this object instance, PropertyInfo propertyInfo, object value)
        {
            SetPropertyValue(instance, propertyInfo.Name, value);
        }

        public static void SetPropertyValue(this object instance, string propertyName, object value)
        {
            var obj = instance;

            string[] bits = propertyName.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = obj.GetType().GetProperty(bits[i]);

                var currentVal = propertyToGet.GetValue(obj, null);

                if (currentVal == null)
                {
                    propertyToGet.SetValue(obj, Activator.CreateInstance(propertyToGet.PropertyType));
                }

                obj = propertyToGet.GetValue(obj, null);
            }

            PropertyInfo propertyToSet = obj.GetType().GetProperty(bits.Last());

            if (propertyToSet.CanWrite)
            {
                propertyToSet.SetValue(obj, value, null);
            }
        }

        public static Guid EncodeAsGuid(this object obj)
        {
            Guid result;
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.Default.GetBytes(obj.ToString()));
                Array.Resize(ref hash, 16); // Needs to be 128 bits max
                result = new Guid(hash);
            }
            return result;
        }
    }
}
