﻿namespace CommerceApiDemo.AdminDto
{
    public class OrderSummaryResponse
    {
        public int OrderCount { get; set; }
        public int ProductCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
