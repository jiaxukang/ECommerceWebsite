using System;
using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specification;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null)
    {
    }

    public Expression<Func<T, bool>> Criteria => criteria!;

    public Expression<Func<T, object>>? OrderBy { get; protected set; }
    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

    

    public bool IsDistinct { get; protected set; }

    public int Take { get; protected set; }

    public int Skip { get; protected set; }

    public bool IsPagingEnabled { get; protected set; }

    public List<Expression<Func<T, object>>> Includes { get; }= [];

    public List<string> IncludeStrings { get; } = [];

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    protected void AddDistinct()
    {
        IsDistinct = true;
    }

    protected void AddPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString); // For ThenInclude
    }

    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }
        return query;
    }
}

public class BaseSpecification<T, TResult>(Expression<Func<T, bool>> criteria)
: BaseSpecification<T>(criteria), ISpecification<T, TResult>
{


    protected BaseSpecification() : this(null!)
    {
    }
    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }
    
    
}
