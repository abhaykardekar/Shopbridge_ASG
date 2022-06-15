alter proc usp_update_productdetails
(
@product_id bigint
,@prod_name nvarchar(200)
,@prod_desc nvarchar(500)
,@unit_price numeric(10,2)
,@sell_price numeric(10,2)=null
,@discount numeric(10,2)=null
,@discount_price numeric(10,2)=null
,@stock_qty bigint=null
,@modifiedby nvarchar(200)
)
as
begin
	if(exists(select 1 from TBL_PRODUCT_MASTER with(nolock) where id=@product_id))
	begin
		update A
		set
		product_name=@prod_name
		,product_desc=@prod_desc
		,unit_price=@unit_price
		,sell_price=@sell_price
		,discount=@discount
		,discount_price=@discount_price				
		,stock_qty=@stock_qty
		,modifiedby=@modifiedby,modifiedon=getdate()
		from TBL_PRODUCT_MASTER A with(nolock)
		where id=@product_id

		select '0' as ErrorCode,'Product updated successfully!' as ErrorDesc
	end
	else
	begin
		select '1' as ErrorCode,'Product not present in inventory to update!' as ErrorDesc
	end
end