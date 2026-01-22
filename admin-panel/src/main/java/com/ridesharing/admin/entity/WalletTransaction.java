package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "WalletTransactions")
public class WalletTransaction {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "WalletID")
    private Integer walletId;
    
    @Column(name = "Amount")
    private BigDecimal amount;
    
    @Column(name = "Type")
    private String type;
    
    @Column(name = "TransactionDate")
    private LocalDateTime transactionDate;
    
    @Column(name = "ReferenceID")
    private Integer referenceId;
    
    @Column(name = "Description")
    private String description;
}
