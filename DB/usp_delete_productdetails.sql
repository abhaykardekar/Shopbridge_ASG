alter proc usp_delete_productdetails
(
@product_id bigint
)
as
begin
	if(exists(select 1 from TBL_PRODUCT_MASTER with(nolock) where id=@product_id))
	begin
		delete from TBL_PRODUCT_MASTER
		where id=@product_id

		select '0' as ErrorCode,'Product deleted successfully!' as ErrorDesc
	end
	else
	begin
		select '1' as ErrorCode,'Product not present in inventory to remove!' as ErrorDesc
	end
end