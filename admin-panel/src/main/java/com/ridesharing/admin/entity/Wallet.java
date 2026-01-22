package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "Wallets")
public class Wallet {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "UserID")
    private Integer userId;
    
    @Column(name = "Balance")
    private BigDecimal balance;
    
    @Column(name = "Currency")
    private String currency;
    
    @Column(name = "UpdatedAt")
    private LocalDateTime updatedAt;
}
