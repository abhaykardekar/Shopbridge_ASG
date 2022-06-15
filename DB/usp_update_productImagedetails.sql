create proc usp_update_productImagedetails
(
@product_id bigint,
@prod_img nvarchar(max),
@modifiedby nvarchar(200)
)
as
begin
	if(exists(select 1 from TBL_PRODUCT_MASTER with(nolock) where id=@product_id))
	begin
		update A
		set A.product_image_path=@prod_img
		from TBL_PRODUCT_MASTER A with(nolock)
		where A.id=@product_id

		select '0' as ErrorCode,'Product image updated successully!' as ErrorDesc
	end
	else
	begin
		select '1' as ErrorCode,'Product not present in inventory to update!' as ErrorDesc
	end
end