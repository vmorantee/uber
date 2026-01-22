package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "Rides")
public class Ride {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "PassengerID")
    private Integer passengerId;
    
    @Column(name = "DriverID")
    private Integer driverId;
    
    @Column(name = "StartLocationLat")
    private Double pickupLat;
    
    @Column(name = "StartLocationLng")
    private Double pickupLng;
    
    @Column(name = "EndLocationLat")
    private Double dropoffLat;
    
    @Column(name = "EndLocationLng")
    private Double dropoffLng;
    
    @Column(name = "StartAddress")
    private String startAddress;
    
    @Column(name = "EndAddress")
    private String endAddress;
    
    @Column(name = "Price")
    private BigDecimal price;
    
    @Column(name = "Distance")
    private BigDecimal distance;
    
    @Column(name = "Status")
    private String status;
    
    @Column(name = "StartTime")
    private LocalDateTime startTime;
    
    @Column(name = "CreatedAt")
    private LocalDateTime createdAt;
    
    // Helper methods for display
    @Transient
    public String getPickupAddress() {
        if (startAddress != null && !startAddress.isEmpty()) {
            return startAddress;
        }
        if (pickupLat != null && pickupLng != null) {
            return String.format("%.4f, %.4f", pickupLat, pickupLng);
        }
        return "N/A";
    }
    
    @Transient
    public String getDropoffAddress() {
        if (endAddress != null && !endAddress.isEmpty()) {
            return endAddress;
        }
        if (dropoffLat != null && dropoffLng != null) {
            return String.format("%.4f, %.4f", dropoffLat, dropoffLng);
        }
        return "N/A";
    }
}
