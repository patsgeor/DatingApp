export type PaginationMetadata ={
    currentPage :number ;
    pageSize :number ;
    totalCount :number;
    totalPages: number;
}

export type PaginatedResult<T> ={
    metadata: PaginationMetadata;
    items:T[];
}
