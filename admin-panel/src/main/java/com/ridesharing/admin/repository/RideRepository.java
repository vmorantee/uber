package com.ridesharing.admin.repository;

import com.ridesharing.admin.entity.Ride;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface RideRepository extends JpaRepository<Ride, Integer> {
    List<Ride> findByStatus(String status);
    
    @Query("SELECT COUNT(r) FROM Ride r WHERE r.status = :status")
    long countByStatus(String status);
    
    @Query("SELECT COALESCE(SUM(r.price), 0) FROM Ride r WHERE r.status = 'Completed'")
    java.math.BigDecimal getTotalRevenue();
    
    List<Ride> findTop10ByOrderByCreatedAtDesc();
}
