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

export class MemberParams {
    pageNumber= 1;
    pageSize= 10;
    minAge=18;
    maxAge=100;
    gender?: string;
    orderBy:string="lastActive"
}


export class LikesParams {
    pageNumber= 1;
    pageSize= 10;
    predicate="liked";
}