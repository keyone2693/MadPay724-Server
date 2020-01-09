using MadPay724.Common.Enums.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MadPay724.Common.Helpers.Utilities.Extensions
{
    public static class CombineExpressions
    {
        public static Expression<Func<T, bool>> CombiningExpressions<T>(Expression<Func<T, bool>> func1, Expression<Func<T, bool>> func2, ExpressionsTypeEnum expressionsType)
        {
            if (expressionsType == ExpressionsTypeEnum.And)
            {
                return func1.CombineWithAndAlso(func2);
            }
            else if (expressionsType == ExpressionsTypeEnum.Or)
            {
                return func1.CombineWithOrElse(func2);
            }
            else
            {
                return null;
            }
        }

        public static Expression<Func<TInput, bool>> CombineWithAndAlso<TInput>(this Expression<Func<TInput, bool>> func1, Expression<Func<TInput, bool>> func2)
        {
            return Expression.Lambda<Func<TInput, bool>>(
                Expression.AndAlso(
                    func1.Body, new ExpressionParameterReplacer(func2.Parameters, func1.Parameters).Visit(func2.Body)),
                func1.Parameters);
        }

        public static Expression<Func<TInput, bool>> CombineWithOrElse<TInput>(this Expression<Func<TInput, bool>> func1, Expression<Func<TInput, bool>> func2)
        {
            return Expression.Lambda<Func<TInput, bool>>(
                Expression.OrElse(
                    func1.Body, new ExpressionParameterReplacer(func2.Parameters, func1.Parameters).Visit(func2.Body)),
                func1.Parameters);
        }

        private class ExpressionParameterReplacer : ExpressionVisitor
        {
            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
            {
                ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
                for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                    ParameterReplacements.Add(fromParameters[i], toParameters[i]);
            }

            private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression replacement;
                if (ParameterReplacements.TryGetValue(node, out replacement))
                    node = replacement;
                return base.VisitParameter(node);
            }
        }
    }
}
