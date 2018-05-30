using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Types
{
    public static class SparkReflector
    {

        public static PropertyInfo GetPropertyInfo<TObjectType>(Expression<Func<TObjectType, object>> expressionThatAccessAMember)
        {
            return typeof(TObjectType).GetProperty(GetPropertyName(expressionThatAccessAMember));

        }

        public static PropertyDescriptor GetPropertyDescriptor<TObjectType>(Expression<Func<TObjectType, object>> expressionThatAccessAMember)
        {
            return TypeDescriptor.GetProperties(typeof(TObjectType))[GetPropertyName(expressionThatAccessAMember)];
        }

        public static PropertyDescriptor GetPropertyDescriptor<TObjectType, TProperty>(Expression<Func<TObjectType, TProperty>> expressionThatAccessAMember)
        {
            return TypeDescriptor.GetProperties(typeof(TObjectType))[GetPropertyName(expressionThatAccessAMember)];
        }

        public static string GetPropertyName<TItemType, TPropertyType>(Expression<Func<TItemType, TPropertyType>> expressionThatAccessAMember)
        {
            if (expressionThatAccessAMember == null)
                throw new ArgumentNullException("No expression provided");

            MemberExpression memberExpression = null;
            if (expressionThatAccessAMember.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expressionThatAccessAMember.Body).Operand as MemberExpression;
            }
            else if (expressionThatAccessAMember.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expressionThatAccessAMember.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("The lamda expression " + expressionThatAccessAMember.ToString() + " is not a member accessor");
            }
            return memberExpression.Member.Name;
        }


        public static string GetPropertyName<T>(Expression<Func<T, object>> expressionThatAccessAMember)
        {
            return GetPropertyName<T, object>(expressionThatAccessAMember);
        }



        public static string GetMethodName<TItemType>(Expression<Func<TItemType, Func<object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static string GetMethodName<TItemType, TReturn>(Expression<Func<TItemType, Func<TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType>(Expression<Func<TItemType, Func<object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[0]);
        }

        public static MethodInfo GetMethodInfo<TItemType, TReturn>(Expression<Func<TItemType, Func<TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[0]);
        }

        public static string GetMethodName<TItemType, P1>(Expression<Func<TItemType, Func<P1, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static string GetMethodName<TItemType, P1, TReturn>(Expression<Func<TItemType, Func<P1, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType, P1>(Expression<Func<TItemType, Func<P1, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1) });
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, TReturn>(Expression<Func<TItemType, Func<P1, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1) });
        }

        public static string GetMethodName<TItemType, P1, P2>(Expression<Func<TItemType, Func<P1, P2, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }


        public static string GetMethodName<TItemType, P1, P2, TReturn>(Expression<Func<TItemType, Func<P1, P2, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, P2>(Expression<Func<TItemType, Func<P1, P2, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2) });
        }


        public static MethodInfo GetMethodInfo<TItemType, P1, P2, TReturn>(Expression<Func<TItemType, Func<P1, P2, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2) });
        }


        public static string GetMethodName<TItemType, P1, P2, P3>(Expression<Func<TItemType, Func<P1, P2, P3, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static string GetMethodName<TItemType, P1, P2, P3, TReturn>(Expression<Func<TItemType, Func<P1, P2, P3, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }


        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3>(Expression<Func<TItemType, Func<P1, P2, P3, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3) });
        }


        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3, TReturn>(Expression<Func<TItemType, Func<P1, P2, P3, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3) });
        }

        public static string GetMethodName<TItemType, P1, P2, P3, P4>(Expression<Func<TItemType, Func<P1, P2, P3, P4, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }


        public static string GetMethodName<TItemType, P1, P2, P3, P4, TReturn>(Expression<Func<TItemType, Func<P1, P2, P3, P4, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }


        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3, P4>(Expression<Func<TItemType, Func<P1, P2, P3, P4, object>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3), typeof(P4) });
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3, P4, TReturn>(Expression<Func<TItemType, Func<P1, P2, P3, P4, TReturn>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3), typeof(P4) });
        }

        public static string GetMethodName<TItemType>(Expression<Func<TItemType, Action>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType>(Expression<Func<TItemType, Action>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[0]);
        }

        public static string GetMethodName<TItemType, P1>(Expression<Func<TItemType, Action<P1>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }


        public static MethodInfo GetMethodInfo<TItemType, P1>(Expression<Func<TItemType, Action<P1>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1) });
        }

        public static string GetMethodName<TItemType, P1, P2>(Expression<Func<TItemType, Action<P1, P2>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, P2>(Expression<Func<TItemType, Action<P1, P2>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2) });
        }

        public static string GetMethodName<TItemType, P1, P2, P3>(Expression<Func<TItemType, Action<P1, P2, P3>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3>(Expression<Func<TItemType, Action<P1, P2, P3>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3) });
        }

        public static string GetMethodName<TItemType, P1, P2, P3, P4>(Expression<Func<TItemType, Action<P1, P2, P3, P4>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodName(expression.Body);
        }

        public static MethodInfo GetMethodInfo<TItemType, P1, P2, P3, P4>(Expression<Func<TItemType, Action<P1, P2, P3, P4>>> expression)
        {

            if (expression == null)
                throw new ArgumentNullException("No expression provided");


            return GetMethodInfo(expression.Body, new Type[] { typeof(P1), typeof(P2), typeof(P3), typeof(P4) });
        }


        private static string GetMethodName(Expression expressionBody)
        {
            return ExtractMethodInfo(expressionBody).Name;
        }



        private static MethodInfo GetMethodInfo(Expression expressionBody, Type[] argumentTypes)
        {
            var mInfo = ExtractMethodInfo(expressionBody);

            return mInfo.DeclaringType.GetMethod(mInfo.Name, argumentTypes);
        }

        private static MethodInfo ExtractMethodInfo(Expression expressionBody)
        {
            var unaryExpression = expressionBody as System.Linq.Expressions.UnaryExpression;
            var methodCallExpression = unaryExpression.Operand as System.Linq.Expressions.MethodCallExpression;

            if (methodCallExpression.Object == null)
            {
                //framework 3.5
                return (MethodInfo)(methodCallExpression.Arguments[2] as System.Linq.Expressions.ConstantExpression).Value;
            }
            else
            {
                //framework 4.0
                return (MethodInfo)(methodCallExpression.Object as System.Linq.Expressions.ConstantExpression).Value;
            }
        }

        public class Resolver
        {
            public string Resolve(Expression expression)
            {
                if (expression.NodeType == ExpressionType.MemberAccess)
                    return ResolveMember(expression as MemberExpression);

                if (expression.NodeType == ExpressionType.Convert)
                    return ResolveUnary(expression as UnaryExpression);

                if (expression.NodeType == ExpressionType.Call)
                    return ResolveCall(expression as MethodCallExpression);

                if (expression.NodeType == ExpressionType.Parameter)
                    return ResolveParameter(expression as ParameterExpression);

                throw new NotSupportedException("The method or operation is not implemented. " + expression.NodeType.ToString());
            }

            public string ResolveMember(MemberExpression expression)
            {
                return Resolve(expression.Expression) + expression.Member.Name;
            }

            public string ResolveUnary(UnaryExpression expr)
            {
                return Resolve(expr.Operand);
            }

            public string ResolveCall(MethodCallExpression expression)
            {
                return Resolve(expression.Object) + ".";
            }

            public string ResolveParameter(ParameterExpression expression)
            {
                return string.Empty;
            }
        }

        public static string GetPropertyFullPath<T>(Expression<Func<T, object>> expr)
        {
            if (expr.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Not a lambda expression");

            return new Resolver().Resolve(expr.Body);
        }

    }


    public static class SparkReflectorExtensions
    {
        public static string GetPropertyName<T>(this T obiectul, Expression<Func<T, object>> proprietate)
        {
            return SparkReflector.GetPropertyName<T>(proprietate);
        }

        public static PropertyDescriptor GetPropertyDescriptor<T>(this T obiectul, Expression<Func<T, object>> proprietate)
        {
            return SparkReflector.GetPropertyDescriptor<T>(proprietate);
        }

    }
}
