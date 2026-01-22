package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "Users")
public class User {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "Email")
    private String email;
    
    @Column(name = "PasswordHash")
    private String passwordHash;
    
    @Column(name = "FirstName")
    private String firstName;
    
    @Column(name = "LastName")
    private String lastName;
    
    @Column(name = "Role")
    private String role;
    
    @Column(name = "IsDriverApproved")
    private Boolean isDriverApproved;
    
    @Column(name = "CurrentContext")
    private String currentContext;
    
    @Column(name = "PhoneNumber")
    private String phoneNumber;
    
    @Column(name = "VehicleModel")
    private String vehicleModel;
    
    @Column(name = "LicensePlate")
    private String licensePlate;
    
    @Column(name = "CreatedAt")
    private LocalDateTime createdAt;
    
    @Column(name = "UpdatedAt")
    private LocalDateTime updatedAt;
}
