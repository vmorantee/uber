package com.ridesharing.admin.repository;

import com.ridesharing.admin.entity.Dispute;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface DisputeRepository extends JpaRepository<Dispute, Integer> {
    List<Dispute> findByStatus(String status);
}
