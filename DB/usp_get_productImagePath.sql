create proc usp_get_productImagePath
(
@product_id bigint
)
as
begin
	select isnull(product_image_path,'') as prod_img_path
	from TBL_PRODUCT_MASTER with(nolock)
	where id=@product_id
end