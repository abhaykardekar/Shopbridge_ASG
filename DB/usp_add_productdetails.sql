create proc usp_add_productdetails
(
@category_id bigint
,@sub_category_id bigint
,@prod_name nvarchar(200)
,@prod_desc nvarchar(500)
,@unit_price numeric(10,2)
,@sell_price numeric(10,2)=null
,@discount numeric(10,2)=null
,@discount_price numeric(10,2)=null
,@prod_img nvarchar(max)=null
,@stock_qty bigint
,@createdby nvarchar(200)
,@modifiedby nvarchar(200)
)
as
begin
	if(not exists(select 1 from TBL_PRODUCT_MASTER with(nolock)
	where category_id=@category_id and sub_category_id=@sub_category_id
	and rtrim(upper(product_name))=rtrim(upper(@prod_name))
	))
	begin
		insert into TBL_PRODUCT_MASTER
		(
		category_id,sub_category_id,product_name,product_desc,unit_price,sell_price
		,discount,discount_price,product_image_path,stock_qty,createdby,createdon
		,modifiedby,modifiedon	
		)
		select @category_id,@sub_category_id,@prod_name,@prod_desc,
		@unit_price,@sell_price,@discount,@discount_price,@prod_img
		,@stock_qty,@createdby,getdate(),@modifiedby,getdate()

		select '0' as ErrorCode,'Product added successfully!' as ErrorDesc
	end
	else
	begin
		select '1' as ErrorCode,'Product already added!' as ErrorDesc
	end
end