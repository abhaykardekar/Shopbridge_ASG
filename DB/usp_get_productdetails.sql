alter proc usp_get_productdetails
(
@category_id bigint=null,
@sub_category_id bigint=null
)
as
begin
	select a.id as product_id
	,a.category_id
	,b.category_name
	,a.sub_category_id
	,c.sub_category_name
	,a.product_name
	,a.product_desc
	,a.unit_price
	,a.sell_price
	,a.discount
	,a.discount_price
	,a.stock_qty	
	from TBL_PRODUCT_MASTER a with(nolock)
	inner join tbl_category_master b with(nolock) on a.category_id=b.id
	left join tbl_sub_category_master c with(nolock) on b.id=c.id
	where a.category_id=isnull(@category_id,a.category_id)
	and a.sub_category_id=isnull(@sub_category_id,a.sub_category_id)
	order by a.category_id,a.sub_category_id,a.product_name
end