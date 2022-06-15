create proc usp_get_categorydetails
as
begin
	select id as category_id,category_name 
	from tbl_category_master with(nolock)
end