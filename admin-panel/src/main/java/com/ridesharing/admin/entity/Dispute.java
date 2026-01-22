package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "Disputes")
public class Dispute {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "RideID")
    private Integer rideId;
    
    @Column(name = "ReportingUserID")
    private Integer reportingUserId;
    
    @Column(name = "Reason")
    private String reason;
    
    @Column(name = "Status")
    private String status;
    
    @Column(name = "ResolutionDecision")
    private String resolutionDecision;
    
    @Column(name = "RefundAmount")
    private BigDecimal refundAmount;
    
    @Column(name = "CreatedAt")
    private LocalDateTime createdAt;
    
    @Column(name = "ResolvedAt")
    private LocalDateTime resolvedAt;
    
    @Column(name = "ResolvedByAdminID")
    private Integer resolvedByAdminId;
}
