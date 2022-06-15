create proc usp_get_subcategorydetails
(
@category_id bigint=null
)
as
begin
	select id as sub_category_id,sub_category_name
	from tbl_sub_category_master with(nolock)
	where category_id=@category_id
	order by category_id,sub_category_name
end



