using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specification;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(SpecProductParas paras)
        : base(
        p =>
        (string.IsNullOrEmpty(paras.Search) || p.Name.ToLower().Contains(paras.Search)) &&
        (paras.Brands.Count == 0 || paras.Brands.Contains(p.Brand)) &&
        (paras.Types.Count == 0 || paras.Types.Contains(p.Type)))
    {
        AddPaging(paras.PageSize * (paras.PageIndex - 1), paras.PageSize);
        switch (paras.Sort)
        {
            case "priceAsc":
                AddOrderBy(p => p.Price);
                break;
            case "priceDesc":
                AddOrderByDescending(p => p.Price);
                break;
            default:
                AddOrderBy(p => p.Name);
                break;
        }
    }
}
