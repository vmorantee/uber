package com.ridesharing.admin.repository;

import com.ridesharing.admin.entity.Wallet;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.Optional;

@Repository
public interface WalletRepository extends JpaRepository<Wallet, Integer> {
    Optional<Wallet> findByUserId(Integer userId);
}
