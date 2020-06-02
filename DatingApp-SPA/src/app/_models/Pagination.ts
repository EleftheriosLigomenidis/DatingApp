export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginationResult<T>{
    result: T; // messages ,users etc
    pagination:Pagination;
}
